using System;
using System.IO;
using System.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace BikeDataProject.Data.GPS.Test.Functional
{
    class Program
    {
        static void Main(string[] args)
        {
            var points = System.Text.Json.JsonSerializer.Deserialize<Point[]>(File.ReadAllText("track_1.json"));
            
            var features = new FeatureCollection();
            features.Add(new Feature(new LineString(points.Select(x => new Coordinate(x.Longitude, x.Latitude)).ToArray()), 
                new AttributesTable()));
            

            var pointsConverted = points.Select<Point, (double longitude, double latitude, double accuracy, long timeOffset)>(p => (p.Longitude, p.Latitude, p.Accuracy, 0)).ToList();

            var filtered = pointsConverted.Filter(toleranceInMeter: 20);

            var coordinates = filtered.Select(x => new Coordinate(x.longitude, x.latitude)).ToArray();
            
            features.Add(new Feature(new LineString(coordinates), new AttributesTable()));

            var geojsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
            File.WriteAllText("track_1.json.geojson", geojsonWriter.Write(features));
        }
    }
}
