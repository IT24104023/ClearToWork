using System.Text.Json;

namespace ClearToWork.Domain.Validators;

/// <summary>
/// Encapsulates GeoJSON schema validation results, error diagnostics, and parsed feature metadata.
/// </summary>
public record GeoJsonValidationResult(
    bool IsValid,
    IReadOnlyList<string> Errors,
    IReadOnlyList<string> Warnings,
    int FeatureCount,
    string? PrimaryGeometryType)
{
    public static GeoJsonValidationResult Success(int featureCount, string? geometryType, IReadOnlyList<string>? warnings = null) =>
        new(true, Array.Empty<string>(), warnings ?? Array.Empty<string>(), featureCount, geometryType);

    public static GeoJsonValidationResult Failure(params string[] errors) =>
        new(false, errors.ToList(), Array.Empty<string>(), 0, null);

    public static GeoJsonValidationResult Failure(IEnumerable<string> errors, IEnumerable<string>? warnings = null) =>
        new(false, errors.ToList(), warnings?.ToList() ?? new List<string>(), 0, null);
}

/// <summary>
/// Validates GeoJSON zone definitions for ClearToWork AI Permit-to-Work GIS mapping.
/// Validates RFC 7946 Feature and FeatureCollection structures, supported geometry types
/// (Polygon, MultiPolygon, Point with radius), coordinate bounds [longitude, latitude],
/// and WGS84 coordinate reference system compliance.
/// </summary>
public class GeoJsonSchemaValidator
{
    private static readonly HashSet<string> AllowedGeometryTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Polygon",
        "MultiPolygon",
        "Point"
    };

    private static readonly HashSet<string> RecognizedWgs84CrsIdentifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        "urn:ogc:def:crs:OGC:1.3:CRS84",
        "urn:ogc:def:crs:EPSG::4326",
        "EPSG:4326",
        "urn:ogc:def:crs:OGC:2:84",
        "CRS84"
    };

    /// <summary>
    /// Validates a raw GeoJSON string payload against ClearToWork zone schema specifications.
    /// </summary>
    /// <param name="geoJson">Raw JSON string.</param>
    /// <returns>A validation result with status and diagnostic details.</returns>
    public GeoJsonValidationResult Validate(string geoJson)
    {
        if (string.IsNullOrWhiteSpace(geoJson))
        {
            return GeoJsonValidationResult.Failure("GeoJSON payload cannot be null, empty, or whitespace.");
        }

        try
        {
            using var doc = JsonDocument.Parse(geoJson);
            return ValidateElement(doc.RootElement);
        }
        catch (JsonException ex)
        {
            return GeoJsonValidationResult.Failure($"Malformed JSON syntax: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates a parsed <see cref="JsonElement"/> representing GeoJSON.
    /// </summary>
    public GeoJsonValidationResult ValidateElement(JsonElement root)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        if (root.ValueKind != JsonValueKind.Object)
        {
            return GeoJsonValidationResult.Failure("GeoJSON root must be a valid JSON Object.");
        }

        // 1. Validate CRS if explicitly specified (RFC 7946 defaults to WGS84, but legacy GeoJSON may include 'crs')
        ValidateCoordinateReferenceSystem(root, errors, warnings);

        // 2. Validate top-level 'type'
        if (!root.TryGetProperty("type", out var typeProp) || typeProp.ValueKind != JsonValueKind.String)
        {
            errors.Add("GeoJSON root object is missing mandatory string property 'type'.");
            return GeoJsonValidationResult.Failure(errors);
        }

        string rootType = typeProp.GetString()!;
        int featureCount = 0;
        string? primaryGeometryType = null;

        switch (rootType)
        {
            case "FeatureCollection":
                if (!root.TryGetProperty("features", out var featuresProp) || featuresProp.ValueKind != JsonValueKind.Array)
                {
                    errors.Add("FeatureCollection must contain a 'features' array property.");
                    return GeoJsonValidationResult.Failure(errors);
                }

                if (featuresProp.GetArrayLength() == 0)
                {
                    warnings.Add("FeatureCollection contains zero features; hazard zone will have no spatial extent.");
                }

                foreach (var featureElement in featuresProp.EnumerateArray())
                {
                    featureCount++;
                    var (featErrors, featGeomType) = ValidateFeature(featureElement, featureCount);
                    errors.AddRange(featErrors);
                    primaryGeometryType ??= featGeomType;
                }
                break;

            case "Feature":
                featureCount = 1;
                var (singleErrors, singleGeomType) = ValidateFeature(root, 1);
                errors.AddRange(singleErrors);
                primaryGeometryType = singleGeomType;
                break;

            default:
                errors.Add($"Unsupported GeoJSON root type '{rootType}'. Only 'Feature' and 'FeatureCollection' are accepted for zone definitions.");
                break;
        }

        return errors.Count == 0
            ? GeoJsonValidationResult.Success(featureCount, primaryGeometryType, warnings)
            : GeoJsonValidationResult.Failure(errors, warnings);
    }

    private static (List<string> Errors, string? GeometryType) ValidateFeature(JsonElement feature, int featureIndex)
    {
        var errors = new List<string>();
        string? geometryType = null;

        if (feature.ValueKind != JsonValueKind.Object)
        {
            errors.Add($"Feature [{featureIndex}] must be an object.");
            return (errors, null);
        }

        if (!feature.TryGetProperty("type", out var typeProp) || typeProp.GetString() != "Feature")
        {
            errors.Add($"Feature [{featureIndex}] must have 'type': 'Feature'.");
        }

        if (!feature.TryGetProperty("geometry", out var geometryProp) || geometryProp.ValueKind == JsonValueKind.Null)
        {
            errors.Add($"Feature [{featureIndex}] is missing mandatory 'geometry' definition.");
            return (errors, null);
        }

        if (geometryProp.ValueKind != JsonValueKind.Object)
        {
            errors.Add($"Feature [{featureIndex}] 'geometry' must be a JSON object.");
            return (errors, null);
        }

        if (!geometryProp.TryGetProperty("type", out var geomTypeProp) || geomTypeProp.ValueKind != JsonValueKind.String)
        {
            errors.Add($"Feature [{featureIndex}] geometry must have a valid 'type' string property.");
            return (errors, null);
        }

        geometryType = geomTypeProp.GetString()!;
        if (!AllowedGeometryTypes.Contains(geometryType))
        {
            errors.Add($"Feature [{featureIndex}] uses unsupported geometry type '{geometryType}'. Allowed: {string.Join(", ", AllowedGeometryTypes)}.");
            return (errors, geometryType);
        }

        if (!geometryProp.TryGetProperty("coordinates", out var coordsProp) || coordsProp.ValueKind != JsonValueKind.Array)
        {
            errors.Add($"Feature [{featureIndex}] geometry '{geometryType}' is missing 'coordinates' array.");
            return (errors, geometryType);
        }

        // Validate coordinates based on geometry type
        switch (geometryType)
        {
            case "Point":
                ValidatePointGeometry(coordsProp, feature, featureIndex, errors);
                break;

            case "Polygon":
                ValidatePolygonCoordinates(coordsProp, featureIndex, errors);
                break;

            case "MultiPolygon":
                ValidateMultiPolygonCoordinates(coordsProp, featureIndex, errors);
                break;
        }

        return (errors, geometryType);
    }

    private static void ValidatePointGeometry(JsonElement coords, JsonElement feature, int index, List<string> errors)
    {
        // Coordinate pair [longitude, latitude]
        ValidateCoordinatePair(coords, $"Feature [{index}] Point", errors);

        // Point zones require a radius in properties or properties.radiusMeters
        bool hasRadius = false;
        if (feature.TryGetProperty("properties", out var props) && props.ValueKind == JsonValueKind.Object)
        {
            if ((props.TryGetProperty("radius", out var rad) || props.TryGetProperty("radiusMeters", out rad))
                && rad.ValueKind == JsonValueKind.Number && rad.GetDouble() > 0)
            {
                hasRadius = true;
            }
        }

        if (!hasRadius)
        {
            errors.Add($"Feature [{index}] Point geometry zone requires a positive numeric 'radius' or 'radiusMeters' property in 'properties'.");
        }
    }

    private static void ValidatePolygonCoordinates(JsonElement coords, int index, List<string> errors)
    {
        if (coords.GetArrayLength() == 0)
        {
            errors.Add($"Feature [{index}] Polygon coordinates array cannot be empty.");
            return;
        }

        // Polygon coordinates is an array of linear rings (first ring is exterior boundary, optional subsequent are holes)
        int ringIndex = 0;
        foreach (var ring in coords.EnumerateArray())
        {
            if (ring.ValueKind != JsonValueKind.Array)
            {
                errors.Add($"Feature [{index}] Polygon linear ring [{ringIndex}] must be an array of coordinates.");
                continue;
            }

            int count = ring.GetArrayLength();
            if (count < 4)
            {
                errors.Add($"Feature [{index}] Polygon linear ring [{ringIndex}] must contain at least 4 positions (minimum 3 vertices + closing position). Found: {count}.");
                continue;
            }

            // Validate all points in the ring
            var positions = new List<(double Lon, double Lat)>();
            int posIndex = 0;
            foreach (var pos in ring.EnumerateArray())
            {
                if (ValidateCoordinatePair(pos, $"Feature [{index}] Polygon ring [{ringIndex}] position [{posIndex}]", errors) is { } pt)
                {
                    positions.Add(pt);
                }
                posIndex++;
            }

            // Verify closure: first position equals last position
            if (positions.Count >= 2)
            {
                var first = positions[0];
                var last = positions[^1];
                if (Math.Abs(first.Lon - last.Lon) > 1e-7 || Math.Abs(first.Lat - last.Lat) > 1e-7)
                {
                    errors.Add($"Feature [{index}] Polygon linear ring [{ringIndex}] is not closed. First position [{first.Lon}, {first.Lat}] does not match last position [{last.Lon}, {last.Lat}].");
                }
            }

            ringIndex++;
        }
    }

    private static void ValidateMultiPolygonCoordinates(JsonElement coords, int index, List<string> errors)
    {
        if (coords.GetArrayLength() == 0)
        {
            errors.Add($"Feature [{index}] MultiPolygon coordinates array cannot be empty.");
            return;
        }

        int polyIndex = 0;
        foreach (var polygonCoords in coords.EnumerateArray())
        {
            if (polygonCoords.ValueKind != JsonValueKind.Array)
            {
                errors.Add($"Feature [{index}] MultiPolygon element [{polyIndex}] must be a valid Polygon coordinate array.");
            }
            else
            {
                ValidatePolygonCoordinates(polygonCoords, index, errors);
            }
            polyIndex++;
        }
    }

    /// <summary>
    /// Validates that a coordinate array represents [longitude, latitude] within valid geographic limits.
    /// </summary>
    private static (double Lon, double Lat)? ValidateCoordinatePair(JsonElement pos, string context, List<string> errors)
    {
        if (pos.ValueKind != JsonValueKind.Array)
        {
            errors.Add($"{context}: Expected coordinate array, but received {pos.ValueKind}.");
            return null;
        }

        int len = pos.GetArrayLength();
        if (len < 2)
        {
            errors.Add($"{context}: Coordinate array must contain at least 2 numbers [longitude, latitude]. Found: {len}.");
            return null;
        }

        if (!pos[0].TryGetDouble(out double lon))
        {
            errors.Add($"{context}: Longitude value at index 0 must be a valid floating-point number.");
            return null;
        }

        if (!pos[1].TryGetDouble(out double lat))
        {
            errors.Add($"{context}: Latitude value at index 1 must be a valid floating-point number.");
            return null;
        }

        // Validate Range: Longitude [-180, 180]
        if (lon is < -180.0 or > 180.0)
        {
            errors.Add($"{context}: Longitude value {lon:F6} is out of valid WGS84 range [-180.0, 180.0].");
        }

        // Validate Range: Latitude [-90, 90]
        if (lat is < -90.0 or > 90.0)
        {
            errors.Add($"{context}: Latitude value {lat:F6} is out of valid WGS84 range [-90.0, 90.0].");
        }

        return (lon, lat);
    }

    /// <summary>
    /// Validates the Coordinate Reference System (CRS) matches WGS84 if explicitly supplied.
    /// </summary>
    private static void ValidateCoordinateReferenceSystem(JsonElement root, List<string> errors, List<string> warnings)
    {
        if (!root.TryGetProperty("crs", out var crsProp))
        {
            // Per RFC 7946 Section 4: "The coordinate reference system for all GeoJSON coordinates is a geographic
            // coordinate reference system, using the World Geodetic System 1984 (WGS 84) datum."
            return;
        }

        if (crsProp.ValueKind != JsonValueKind.Object)
        {
            warnings.Add("Invalid 'crs' property format; expected object. Standard WGS84 datum will be assumed.");
            return;
        }

        if (crsProp.TryGetProperty("properties", out var props) &&
            props.TryGetProperty("name", out var nameProp) &&
            nameProp.ValueKind == JsonValueKind.String)
        {
            string crsName = nameProp.GetString()!;
            if (!RecognizedWgs84CrsIdentifiers.Contains(crsName))
            {
                errors.Add($"Unsupported Coordinate Reference System '{crsName}'. ClearToWork requires WGS84 (EPSG:4326 or CRS84).");
            }
        }
        else
        {
            warnings.Add("Unrecognized 'crs' properties structure. Ensure coordinates conform to standard WGS84 longitude/latitude.");
        }
    }
}
