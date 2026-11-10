using NetTopologySuite.Geometries;

namespace AgriStress.Infrastructure.Utilities;

public static class SpatialUtils
{
    private static readonly GeometryFactory Wgs84Factory = new(new PrecisionModel(), 4326);

    public static Point CreatePointWgs84(double latitude, double longitude)
    {
        // NetTopologySuite: Coordinate(x: longitude, y: latitude)
        return Wgs84Factory.CreatePoint(new Coordinate(longitude, latitude));
    }
}
