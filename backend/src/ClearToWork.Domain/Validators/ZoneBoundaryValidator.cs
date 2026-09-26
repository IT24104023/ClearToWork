namespace ClearToWork.Domain.Validators;

/// <summary>
/// Geographic coordinate representing latitude and longitude in decimal degrees (WGS84).
/// </summary>
public readonly record struct GeoPoint(double Latitude, double Longitude)
{
    /// <summary>
    /// Validates whether the coordinate values are within standard planetary bounds (-90 to 90 lat, -180 to 180 lon).
    /// </summary>
    public bool IsValid() =>
        Latitude is >= -90.0 and <= 90.0 &&
        Longitude is >= -180.0 and <= 180.0;

    public override string ToString() => $"({Latitude:F6}, {Longitude:F6})";
}

/// <summary>
/// Defines the geographical envelope of an oil &amp; gas industrial asset or facility site.
/// </summary>
public record FacilityBounds(
    double MinLatitude,
    double MaxLatitude,
    double MinLongitude,
    double MaxLongitude,
    string FacilityName = "Main Processing Facility")
{
    /// <summary>
    /// Checks whether a given geographic coordinate falls strictly inside the facility envelope.
    /// </summary>
    public bool Contains(GeoPoint point) =>
        point.Latitude >= MinLatitude && point.Latitude <= MaxLatitude &&
        point.Longitude >= MinLongitude && point.Longitude <= MaxLongitude;

    /// <summary>
    /// Default bounds representing a typical offshore production platform complex.
    /// </summary>
    public static FacilityBounds DefaultOffshoreComplex =>
        new(24.5000, 24.6500, 54.3000, 54.4500, "Al-Shaheen Marine Complex");
}

/// <summary>
/// Encapsulates the results of zone boundary validation including error and warning diagnostics.
/// </summary>
public record ZoneBoundaryValidationResult(
    bool IsValid,
    IReadOnlyList<string> Errors,
    IReadOnlyList<string> Warnings)
{
    public static ZoneBoundaryValidationResult Success(IReadOnlyList<string>? warnings = null) =>
        new(true, Array.Empty<string>(), warnings ?? Array.Empty<string>());

    public static ZoneBoundaryValidationResult Failure(params string[] errors) =>
        new(false, errors.ToList(), Array.Empty<string>());

    public static ZoneBoundaryValidationResult Failure(IEnumerable<string> errors, IEnumerable<string>? warnings = null) =>
        new(false, errors.ToList(), warnings?.ToList() ?? new List<string>());
}

/// <summary>
/// Validates hazard zone geometries for Permit-to-Work SIMOPS management.
/// Ensures polygon vertex integrity, self-intersection avoidance, radius safety limits,
/// and facility bounding perimeter compliance.
/// </summary>
public class ZoneBoundaryValidator
{
    public const double MinRadiusMeters = 1.0;
    public const double MaxRadiusMeters = 5000.0;
    public const int MinPolygonPoints = 3;

    private readonly FacilityBounds _facilityBounds;

    public ZoneBoundaryValidator(FacilityBounds? facilityBounds = null)
    {
        _facilityBounds = facilityBounds ?? FacilityBounds.DefaultOffshoreComplex;
    }

    /// <summary>
    /// Validates a circular hazard zone defined by a center coordinate and a radius.
    /// </summary>
    /// <param name="center">Center point of the hazard perimeter.</param>
    /// <param name="radiusMeters">Effective radius in meters (1m to 5000m allowed).</param>
    /// <returns>Validation result indicating pass/fail with error diagnostics.</returns>
    public ZoneBoundaryValidationResult ValidateCircularZone(GeoPoint center, double radiusMeters)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // 1. Coordinate valid bounds check
        if (!center.IsValid())
        {
            errors.Add($"Center coordinate {center} is outside planetary latitude [-90, 90] or longitude [-180, 180] bounds.");
        }

        // 2. Facility bounds verification
        if (!_facilityBounds.Contains(center))
        {
            errors.Add($"Zone center {center} falls outside the authorized facility perimeter '{_facilityBounds.FacilityName}'.");
        }

        // 3. Radius limits validation
        if (radiusMeters < MinRadiusMeters)
        {
            errors.Add($"Zone radius {radiusMeters:F1}m is below minimum permissible safety threshold ({MinRadiusMeters:F1}m).");
        }
        else if (radiusMeters > MaxRadiusMeters)
        {
            errors.Add($"Zone radius {radiusMeters:F1}m exceeds maximum facility containment limit ({MaxRadiusMeters:F1}m).");
        }

        if (radiusMeters > 500.0)
        {
            warnings.Add($"Wide hazard radius ({radiusMeters:F1}m) will trigger extensive SIMOPS exclusion alerts across adjacent modules.");
        }

        return errors.Count == 0
            ? ZoneBoundaryValidationResult.Success(warnings)
            : ZoneBoundaryValidationResult.Failure(errors, warnings);
    }

    /// <summary>
    /// Validates a polygonal hazard zone boundary represented by a sequence of coordinates.
    /// </summary>
    /// <param name="coordinates">List of coordinates forming the boundary vertices.</param>
    /// <returns>Validation result indicating geometric consistency and facility compliance.</returns>
    public ZoneBoundaryValidationResult ValidatePolygon(IReadOnlyList<GeoPoint> coordinates)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        if (coordinates == null || coordinates.Count == 0)
        {
            return ZoneBoundaryValidationResult.Failure("Coordinate array cannot be null or empty.");
        }

        // Deduplicate closing point if explicitly provided
        var vertices = new List<GeoPoint>(coordinates);
        if (vertices.Count >= 2 && ArePointsEqual(vertices[0], vertices[^1]))
        {
            vertices.RemoveAt(vertices.Count - 1);
        }

        // 1. Minimum vertices check
        if (vertices.Count < MinPolygonPoints)
        {
            errors.Add($"Polygonal hazard boundary requires at least {MinPolygonPoints} distinct vertices; received {vertices.Count}.");
            return ZoneBoundaryValidationResult.Failure(errors);
        }

        // 2. Validate each vertex coordinates and facility boundary
        for (int i = 0; i < vertices.Count; i++)
        {
            var pt = vertices[i];
            if (!pt.IsValid())
            {
                errors.Add($"Vertex [{i}] {pt} has invalid latitude or longitude.");
            }
            else if (!_facilityBounds.Contains(pt))
            {
                errors.Add($"Vertex [{i}] {pt} falls outside facility bounds '{_facilityBounds.FacilityName}'.");
            }
        }

        // 3. Check for duplicate adjacent vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            var next = vertices[(i + 1) % vertices.Count];
            if (ArePointsEqual(vertices[i], next))
            {
                warnings.Add($"Consecutive duplicate vertices detected at index {i}. Points should be pruned.");
            }
        }

        // 4. Check for self-intersecting edges (Jordan Curve violation)
        if (HasSelfIntersections(vertices, out var intersectionDetails))
        {
            errors.Add($"Polygon boundary is self-intersecting: {intersectionDetails}");
        }

        // 5. Check for degenerate polygon (near zero area or collinear vertices)
        double area = CalculateSignedPolygonArea(vertices);
        if (Math.Abs(area) < 1e-11)
        {
            errors.Add("Polygon vertices are degenerate or collinear (enclosed surface area is approximately zero).");
        }

        return errors.Count == 0
            ? ZoneBoundaryValidationResult.Success(warnings)
            : ZoneBoundaryValidationResult.Failure(errors, warnings);
    }

    /// <summary>
    /// Evaluates whether any non-adjacent edges of the polygon intersect each other.
    /// </summary>
    private static bool HasSelfIntersections(IReadOnlyList<GeoPoint> vertices, out string details)
    {
        details = string.Empty;
        int n = vertices.Count;

        for (int i = 0; i < n; i++)
        {
            var p1 = vertices[i];
            var p2 = vertices[(i + 1) % n];

            // Compare with other segments that do not share a vertex with segment (p1, p2)
            for (int j = i + 1; j < n; j++)
            {
                // Adjacent segments share a vertex; also the first and last segments share vertex 0
                if (Math.Abs(i - j) <= 1 || (i == 0 && j == n - 1))
                {
                    continue;
                }

                var q1 = vertices[j];
                var q2 = vertices[(j + 1) % n];

                if (DoSegmentsIntersect(p1, p2, q1, q2))
                {
                    details = $"Edge {i}->{(i + 1) % n} intersects edge {j}->{(j + 1) % n}.";
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Tests line segment intersection using standard 2D vector cross product orientations.
    /// </summary>
    private static bool DoSegmentsIntersect(GeoPoint a1, GeoPoint a2, GeoPoint b1, GeoPoint b2)
    {
        int o1 = Orientation(a1, a2, b1);
        int o2 = Orientation(a1, a2, b2);
        int o3 = Orientation(b1, b2, a1);
        int o4 = Orientation(b1, b2, a2);

        // General intersection case
        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        // Collinear special cases
        if (o1 == 0 && OnSegment(a1, b1, a2)) return true;
        if (o2 == 0 && OnSegment(a1, b2, a2)) return true;
        if (o3 == 0 && OnSegment(b1, a1, b2)) return true;
        if (o4 == 0 && OnSegment(b1, a2, b2)) return true;

        return false;
    }

    private static int Orientation(GeoPoint p, GeoPoint q, GeoPoint r)
    {
        double val = (q.Latitude - p.Latitude) * (r.Longitude - q.Longitude) -
                     (q.Longitude - p.Longitude) * (r.Latitude - q.Latitude);

        if (Math.Abs(val) < 1e-9) return 0; // Collinear
        return (val > 0) ? 1 : 2;           // 1: Clockwise, 2: Counterclockwise
    }

    private static bool OnSegment(GeoPoint p, GeoPoint q, GeoPoint r)
    {
        return q.Longitude <= Math.Max(p.Longitude, r.Longitude) &&
               q.Longitude >= Math.Min(p.Longitude, r.Longitude) &&
               q.Latitude <= Math.Max(p.Latitude, r.Latitude) &&
               q.Latitude >= Math.Min(p.Latitude, r.Latitude);
    }

    private static bool ArePointsEqual(GeoPoint a, GeoPoint b) =>
        Math.Abs(a.Latitude - b.Latitude) < 1e-7 &&
        Math.Abs(a.Longitude - b.Longitude) < 1e-7;

    private static double CalculateSignedPolygonArea(IReadOnlyList<GeoPoint> vertices)
    {
        double area = 0.0;
        int n = vertices.Count;
        for (int i = 0; i < n; i++)
        {
            var p1 = vertices[i];
            var p2 = vertices[(i + 1) % n];
            area += (p1.Longitude * p2.Latitude) - (p2.Longitude * p1.Latitude);
        }
        return area / 2.0;
    }
}
