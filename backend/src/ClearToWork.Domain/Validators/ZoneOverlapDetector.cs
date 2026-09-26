using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Validators;

/// <summary>
/// Type of spatial representation for a hazard zone.
/// </summary>
public enum ZoneGeometryType
{
    RadialCircle,
    Polygon
}

/// <summary>
/// Represents spatial footprint for a hazard zone or permit boundary.
/// </summary>
public class ZoneGeometry
{
    public ZoneGeometryType GeometryType { get; init; }
    public GeoPoint Center { get; init; }
    public double RadiusMeters { get; init; }
    public IReadOnlyList<GeoPoint> Vertices { get; init; } = Array.Empty<GeoPoint>();

    public static ZoneGeometry CreateCircle(GeoPoint center, double radiusMeters) =>
        new()
        {
            GeometryType = ZoneGeometryType.RadialCircle,
            Center = center,
            RadiusMeters = radiusMeters
        };

    public static ZoneGeometry CreatePolygon(IEnumerable<GeoPoint> vertices)
    {
        var list = vertices.ToList();
        // Compute approximate centroid as center
        double avgLat = list.Average(p => p.Latitude);
        double avgLon = list.Average(p => p.Longitude);

        return new ZoneGeometry
        {
            GeometryType = ZoneGeometryType.Polygon,
            Center = new GeoPoint(avgLat, avgLon),
            Vertices = list
        };
    }
}

/// <summary>
/// Detailed evaluation of spatial overlap between two hazard zone boundaries.
/// </summary>
public record ZoneOverlapResult(
    bool HasOverlap,
    double OverlapPercentageZoneA,
    double OverlapPercentageZoneB,
    double ApproximateOverlapAreaSqMeters,
    string Details
);

/// <summary>
/// Metadata for an active permit evaluated during spatial conflict detection.
/// </summary>
public record PermitSpatialDescriptor(
    Guid PermitId,
    string PermitNumber,
    string Title,
    string HazardCode,
    HazardSeverity Severity,
    ZoneGeometry Geometry
);

/// <summary>
/// Spatial overlap impact on a specific permit.
/// </summary>
public record PermitOverlapImpact(
    string PermitNumber,
    string Title,
    string HazardCode,
    HazardSeverity Severity,
    double OverlapPercentageWithTarget,
    double OverlapAreaSqMeters,
    bool RequiresSimopsLock
);

/// <summary>
/// Comprehensive summary of all active permits whose hazard perimeters intersect the query zone.
/// </summary>
public record PermitZoneOverlapReport(
    string QueryZoneIdentifier,
    bool HasAnyOverlap,
    HazardSeverity PeakInteractingSeverity,
    IReadOnlyList<PermitOverlapImpact> AffectedPermits
);

/// <summary>
/// High-performance spatial analysis engine for industrial Permit-to-Work systems.
/// Performs point-in-polygon tests (ray casting), polygon collision detection,
/// circle-polygon hybrid intersections, and overlap area percentage estimations.
/// </summary>
public class ZoneOverlapDetector
{
    private const double EarthRadiusMeters = 6371008.8;

    /// <summary>
    /// Performs standard Ray-Casting (Jordan Curve Theorem) to determine if a point is inside a polygon.
    /// </summary>
    /// <param name="point">The geographic point to test.</param>
    /// <param name="polygon">Ordered list of polygon vertices.</param>
    /// <returns>True if the point lies strictly inside or on the boundary.</returns>
    public bool IsPointInPolygon(GeoPoint point, IReadOnlyList<GeoPoint> polygon)
    {
        if (polygon == null || polygon.Count < 3) return false;

        bool inside = false;
        int n = polygon.Count;

        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            double xi = polygon[i].Longitude, yi = polygon[i].Latitude;
            double xj = polygon[j].Longitude, yj = polygon[j].Latitude;

            // Check if point is on vertex or horizontal edge
            if (Math.Abs(point.Latitude - yi) < 1e-9 && Math.Abs(point.Longitude - xi) < 1e-9)
            {
                return true;
            }

            bool intersect = ((yi > point.Latitude) != (yj > point.Latitude)) &&
                             (point.Longitude < (xj - xi) * (point.Latitude - yi) / (yj - yi + 1e-12) + xi);

            if (intersect)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    /// <summary>
    /// Evaluates spatial collision and calculates overlap metrics between two zone geometries.
    /// Supports Circle-to-Circle, Polygon-to-Polygon, and hybrid Circle-to-Polygon topologies.
    /// </summary>
    public ZoneOverlapResult CheckOverlap(ZoneGeometry zoneA, ZoneGeometry zoneB)
    {
        ArgumentNullException.ThrowIfNull(zoneA);
        ArgumentNullException.ThrowIfNull(zoneB);

        // Case 1: Both zones are circular
        if (zoneA.GeometryType == ZoneGeometryType.RadialCircle && zoneB.GeometryType == ZoneGeometryType.RadialCircle)
        {
            return EvaluateCircleCircleOverlap(zoneA, zoneB);
        }

        // Case 2: One is circle, one is polygon
        if (zoneA.GeometryType == ZoneGeometryType.RadialCircle && zoneB.GeometryType == ZoneGeometryType.Polygon)
        {
            return EvaluateCirclePolygonOverlap(zoneA, zoneB, invertOrder: false);
        }

        if (zoneA.GeometryType == ZoneGeometryType.Polygon && zoneB.GeometryType == ZoneGeometryType.RadialCircle)
        {
            return EvaluateCirclePolygonOverlap(zoneB, zoneA, invertOrder: true);
        }

        // Case 3: Both are polygons
        return EvaluatePolygonPolygonOverlap(zoneA, zoneB);
    }

    /// <summary>
    /// Scans a collection of active permits and identifies all operations whose safety zones
    /// encroach into the target zone footprint.
    /// </summary>
    public PermitZoneOverlapReport DetectAffectedPermits(
        string targetZoneCode,
        ZoneGeometry targetZoneGeometry,
        IEnumerable<PermitSpatialDescriptor> activePermits)
    {
        ArgumentNullException.ThrowIfNull(targetZoneGeometry);
        ArgumentNullException.ThrowIfNull(activePermits);

        var affected = new List<PermitOverlapImpact>();
        var peakSeverity = HazardSeverity.None;

        foreach (var permit in activePermits)
        {
            var overlap = CheckOverlap(targetZoneGeometry, permit.Geometry);
            if (overlap.HasOverlap)
            {
                bool requiresLock = overlap.OverlapPercentageZoneA >= 10.0 ||
                                    permit.Severity >= HazardSeverity.High;

                affected.Add(new PermitOverlapImpact(
                    PermitNumber: permit.PermitNumber,
                    Title: permit.Title,
                    HazardCode: permit.HazardCode,
                    Severity: permit.Severity,
                    OverlapPercentageWithTarget: overlap.OverlapPercentageZoneA,
                    OverlapAreaSqMeters: overlap.ApproximateOverlapAreaSqMeters,
                    RequiresSimopsLock: requiresLock
                ));

                if (permit.Severity > peakSeverity)
                {
                    peakSeverity = permit.Severity;
                }
            }
        }

        return new PermitZoneOverlapReport(
            QueryZoneIdentifier: targetZoneCode,
            HasAnyOverlap: affected.Count > 0,
            PeakInteractingSeverity: peakSeverity,
            AffectedPermits: affected.OrderByDescending(a => a.Severity).ThenByDescending(a => a.OverlapPercentageWithTarget).ToList()
        );
    }

    private static ZoneOverlapResult EvaluateCircleCircleOverlap(ZoneGeometry a, ZoneGeometry b)
    {
        double distanceMeters = CalculateHaversineDistance(a.Center, b.Center);
        double r1 = a.RadiusMeters;
        double r2 = b.RadiusMeters;

        // Disjoint (no overlap)
        if (distanceMeters >= r1 + r2)
        {
            return new ZoneOverlapResult(false, 0.0, 0.0, 0.0, "Radial zones are completely disjoint (distance exceeds sum of radii).");
        }

        double areaA = Math.PI * r1 * r1;
        double areaB = Math.PI * r2 * r2;

        // One circle fully inside the other
        if (distanceMeters <= Math.Abs(r1 - r2))
        {
            double containedArea = Math.PI * Math.Pow(Math.Min(r1, r2), 2);
            double pctA = Math.Min(100.0, (containedArea / areaA) * 100.0);
            double pctB = Math.Min(100.0, (containedArea / areaB) * 100.0);
            return new ZoneOverlapResult(true, pctA, pctB, containedArea, "One hazard zone is completely contained inside the other.");
        }

        // Partial circle-circle intersection (lens formula)
        double d = distanceMeters;
        double part1 = r1 * r1 * Math.Acos((d * d + r1 * r1 - r2 * r2) / (2 * d * r1));
        double part2 = r2 * r2 * Math.Acos((d * d + r2 * r2 - r1 * r1) / (2 * d * r2));
        double part3 = 0.5 * Math.Sqrt((-d + r1 + r2) * (d + r1 - r2) * (d - r1 + r2) * (d + r1 + r2));

        double overlapArea = Math.Max(0.0, part1 + part2 - part3);
        double overlapPctA = Math.Min(100.0, Math.Round((overlapArea / areaA) * 100.0, 1));
        double overlapPctB = Math.Min(100.0, Math.Round((overlapArea / areaB) * 100.0, 1));

        return new ZoneOverlapResult(
            true,
            overlapPctA,
            overlapPctB,
            Math.Round(overlapArea, 1),
            $"Circular zones overlap by {overlapArea:F1} m² (Center separation: {d:F1}m)."
        );
    }

    private ZoneOverlapResult EvaluateCirclePolygonOverlap(ZoneGeometry circle, ZoneGeometry polygon, bool invertOrder)
    {
        // 1. Fast check: is circle center inside polygon?
        bool centerInPolygon = IsPointInPolygon(circle.Center, polygon.Vertices);

        // 2. Minimum distance from circle center to any polygon segment
        double minDistance = double.MaxValue;
        int n = polygon.Vertices.Count;
        for (int i = 0; i < n; i++)
        {
            var p1 = polygon.Vertices[i];
            var p2 = polygon.Vertices[(i + 1) % n];
            double dist = DistanceFromPointToSegmentMeters(circle.Center, p1, p2);
            if (dist < minDistance) minDistance = dist;
        }

        bool hasOverlap = centerInPolygon || minDistance <= circle.RadiusMeters;
        if (!hasOverlap)
        {
            return new ZoneOverlapResult(false, 0.0, 0.0, 0.0, "Circular zone perimeter does not intersect polygon boundary.");
        }

        // Estimate overlap area via numerical grid sampling
        double circleArea = Math.PI * circle.RadiusMeters * circle.RadiusMeters;
        double polygonArea = CalculateApproximatePolygonArea(polygon.Vertices);

        double overlapArea = EstimateHybridIntersectionArea(circle, polygon.Vertices);
        double pctCircle = Math.Min(100.0, Math.Round((overlapArea / Math.Max(1.0, circleArea)) * 100.0, 1));
        double pctPoly = Math.Min(100.0, Math.Round((overlapArea / Math.Max(1.0, polygonArea)) * 100.0, 1));

        return invertOrder
            ? new ZoneOverlapResult(true, pctPoly, pctCircle, overlapArea, $"Polygon and circle overlap by approx {overlapArea:F1} m².")
            : new ZoneOverlapResult(true, pctCircle, pctPoly, overlapArea, $"Circle and polygon overlap by approx {overlapArea:F1} m².");
    }

    private ZoneOverlapResult EvaluatePolygonPolygonOverlap(ZoneGeometry a, ZoneGeometry b)
    {
        var polyA = a.Vertices;
        var polyB = b.Vertices;

        // 1. Check if any vertex of A is in B or B in A
        bool aInB = polyA.Any(pt => IsPointInPolygon(pt, polyB));
        bool bInA = polyB.Any(pt => IsPointInPolygon(pt, polyA));

        // 2. Check if any edge intersects
        bool edgesIntersect = DoPolygonsIntersect(polyA, polyB);

        bool hasOverlap = aInB || bInA || edgesIntersect;
        if (!hasOverlap)
        {
            return new ZoneOverlapResult(false, 0.0, 0.0, 0.0, "Polygons are non-overlapping and disjoint.");
        }

        double areaA = CalculateApproximatePolygonArea(polyA);
        double areaB = CalculateApproximatePolygonArea(polyB);

        double overlapArea = EstimatePolygonIntersectionArea(polyA, polyB);
        double pctA = Math.Min(100.0, Math.Round((overlapArea / Math.Max(1.0, areaA)) * 100.0, 1));
        double pctB = Math.Min(100.0, Math.Round((overlapArea / Math.Max(1.0, areaB)) * 100.0, 1));

        return new ZoneOverlapResult(
            true,
            pctA,
            pctB,
            overlapArea,
            $"Polygon zones intersect with approximately {overlapArea:F1} m² shared boundary."
        );
    }

    private static bool DoPolygonsIntersect(IReadOnlyList<GeoPoint> polyA, IReadOnlyList<GeoPoint> polyB)
    {
        int na = polyA.Count;
        int nb = polyB.Count;

        for (int i = 0; i < na; i++)
        {
            var a1 = polyA[i];
            var a2 = polyA[(i + 1) % na];

            for (int j = 0; j < nb; j++)
            {
                var b1 = polyB[j];
                var b2 = polyB[(j + 1) % nb];

                if (LineSegmentsCross(a1, a2, b1, b2))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool LineSegmentsCross(GeoPoint a1, GeoPoint a2, GeoPoint b1, GeoPoint b2)
    {
        int d1 = Direction(b1, b2, a1);
        int d2 = Direction(b1, b2, a2);
        int d3 = Direction(a1, a2, b1);
        int d4 = Direction(a1, a2, b2);

        return (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
                ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)));
    }

    private static int Direction(GeoPoint pi, GeoPoint pj, GeoPoint pk)
    {
        double val = (pk.Longitude - pi.Longitude) * (pj.Latitude - pi.Latitude) -
                     (pj.Longitude - pi.Longitude) * (pk.Latitude - pi.Latitude);
        if (Math.Abs(val) < 1e-9) return 0;
        return val > 0 ? 1 : -1;
    }

    public static double CalculateHaversineDistance(GeoPoint p1, GeoPoint p2)
    {
        double dLat = ToRadians(p2.Latitude - p1.Latitude);
        double dLon = ToRadians(p2.Longitude - p1.Longitude);

        double lat1 = ToRadians(p1.Latitude);
        double lat2 = ToRadians(p2.Latitude);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMeters * c;
    }

    private static double DistanceFromPointToSegmentMeters(GeoPoint p, GeoPoint a, GeoPoint b)
    {
        double l2 = Math.Pow(b.Longitude - a.Longitude, 2) + Math.Pow(b.Latitude - a.Latitude, 2);
        if (l2 < 1e-12) return CalculateHaversineDistance(p, a);

        double t = Math.Max(0, Math.Min(1,
            ((p.Longitude - a.Longitude) * (b.Longitude - a.Longitude) +
             (p.Latitude - a.Latitude) * (b.Latitude - a.Latitude)) / l2));

        var projection = new GeoPoint(
            a.Latitude + t * (b.Latitude - a.Latitude),
            a.Longitude + t * (b.Longitude - a.Longitude));

        return CalculateHaversineDistance(p, projection);
    }

    private static double CalculateApproximatePolygonArea(IReadOnlyList<GeoPoint> vertices)
    {
        if (vertices.Count < 3) return 0.0;
        double area = 0.0;
        int n = vertices.Count;

        // Shoelace formula in projected spherical meters
        double refLat = vertices[0].Latitude;
        double metersPerDegLat = 111132.92;
        double metersPerDegLon = 111412.84 * Math.Cos(ToRadians(refLat));

        for (int i = 0; i < n; i++)
        {
            var p1 = vertices[i];
            var p2 = vertices[(i + 1) % n];

            double x1 = p1.Longitude * metersPerDegLon;
            double y1 = p1.Latitude * metersPerDegLat;
            double x2 = p2.Longitude * metersPerDegLon;
            double y2 = p2.Latitude * metersPerDegLat;

            area += (x1 * y2) - (x2 * y1);
        }

        return Math.Abs(area) / 2.0;
    }

    private double EstimateHybridIntersectionArea(ZoneGeometry circle, IReadOnlyList<GeoPoint> polygon)
    {
        // 20x20 grid sampling over circle bounding box
        int samples = 20;
        double r = circle.RadiusMeters;
        double deltaLat = (r / EarthRadiusMeters) * (180.0 / Math.PI);
        double deltaLon = deltaLat / Math.Cos(ToRadians(circle.Center.Latitude));

        int hits = 0;
        int total = 0;

        for (int i = 0; i < samples; i++)
        {
            double lat = (circle.Center.Latitude - deltaLat) + (2 * deltaLat * i / samples);
            for (int j = 0; j < samples; j++)
            {
                double lon = (circle.Center.Longitude - deltaLon) + (2 * deltaLon * j / samples);
                var pt = new GeoPoint(lat, lon);

                if (CalculateHaversineDistance(circle.Center, pt) <= r)
                {
                    total++;
                    if (IsPointInPolygon(pt, polygon))
                    {
                        hits++;
                    }
                }
            }
        }

        double circleArea = Math.PI * r * r;
        return total > 0 ? (hits / (double)total) * circleArea : 0.0;
    }

    private double EstimatePolygonIntersectionArea(IReadOnlyList<GeoPoint> polyA, IReadOnlyList<GeoPoint> polyB)
    {
        double minLat = Math.Max(polyA.Min(p => p.Latitude), polyB.Min(p => p.Latitude));
        double maxLat = Math.Min(polyA.Max(p => p.Latitude), polyB.Max(p => p.Latitude));
        double minLon = Math.Max(polyA.Min(p => p.Longitude), polyB.Min(p => p.Longitude));
        double maxLon = Math.Min(polyA.Max(p => p.Longitude), polyB.Max(p => p.Longitude));

        if (minLat >= maxLat || minLon >= maxLon) return 0.0;

        int samples = 20;
        int hits = 0;

        for (int i = 0; i < samples; i++)
        {
            double lat = minLat + (maxLat - minLat) * (i + 0.5) / samples;
            for (int j = 0; j < samples; j++)
            {
                double lon = minLon + (maxLon - minLon) * (j + 0.5) / samples;
                var pt = new GeoPoint(lat, lon);

                if (IsPointInPolygon(pt, polyA) && IsPointInPolygon(pt, polyB))
                {
                    hits++;
                }
            }
        }

        double metersPerDegLat = 111132.92;
        double metersPerDegLon = 111412.84 * Math.Cos(ToRadians((minLat + maxLat) / 2.0));
        double boxArea = (maxLat - minLat) * metersPerDegLat * (maxLon - minLon) * metersPerDegLon;

        return (hits / (double)(samples * samples)) * boxArea;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);
}
