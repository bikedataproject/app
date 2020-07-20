using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("BikeDataProject.Data.GPS.Test")]
namespace BikeDataProject.Data.GPS
{
    internal static class RamerDouglasPeucker
    {
        private static readonly IReadOnlyList<(double longitude, double latitude, long timeOffset)> EmptyList = 
            new List<(double longitude, double latitude, long timeOffset)>(0);
        
        internal static IReadOnlyList<(double longitude, double latitude, long timeOffset)> Run(
            this IReadOnlyList<(double longitude, double latitude, long timeOffset)> track,
            double tolerance = 10)
        {
            if (track.Count == 0) return EmptyList;
            
            return track.Run(0, track.Count - 1, tolerance);
        }
        
        internal static IReadOnlyList<(double longitude, double latitude, long timeOffset)> Run(
            this IReadOnlyList<(double longitude, double latitude, long timeOffset)> track,
            int start, int end,
            double tolerance = 10)
        {
            if (start > end) throw new ArgumentOutOfRangeException();
            if (start == end) return new List<(double longitude, double latitude, long timeOffset)>
            {
                (track[start].longitude, track[start].latitude, track[start].timeOffset)
            };
            if (start == end - 1) return new List<(double longitude, double latitude, long timeOffset)>
            {
                (track[start].longitude, track[start].latitude, track[start].timeOffset),
                (track[start+1].longitude, track[start+1].latitude, track[start+1].timeOffset)
            };
            
            // find the point farthest from the line formed between (start -> end).
            var startLocation = (track[start].longitude, track[start].latitude);
            var endLocation = (track[end].longitude, track[end].latitude);

            var farthest = -1;
            var farthestDistance = 0.0;
            if (startLocation.DistanceEstimateInMeter(endLocation) < tolerance)
            {
                // treat as a single location.
                var center = ((startLocation.longitude + endLocation.longitude) / 2,
                    (startLocation.latitude + endLocation.latitude) / 2);
                for (var i = start + 1; i < end - 1; i++)
                {
                    var current =(track[i].longitude, track[i].latitude);
                    var distance = center.DistanceEstimateInMeter(current);
                    if (!(distance > farthestDistance)) continue;
                    
                    farthestDistance = distance;
                    farthest = i;
                }
            }
            else
            {
                // treat as a line.
                var line = (startLocation, endLocation);
                for (var i = start + 1; i < end - 1; i++)
                {
                    var current =(track[i].longitude, track[i].latitude);
                    var  projected = line.ProjectOn(current);
                    if (projected == null) continue;
                    
                    var distance = projected.Value.DistanceEstimateInMeter(current);
                    if (!(distance > farthestDistance)) continue;
                    
                    farthestDistance = distance;
                    farthest = i;
                }
            }

            if (farthestDistance < tolerance ||
                farthest < 0)
            {
                return new List<(double longitude, double latitude, long timeOffset)>
                {
                    (track[start].longitude, track[start].latitude, track[start].timeOffset),
                    (track[end].longitude, track[end].latitude, track[end].timeOffset)
                };
            }
            
            var result = new List<(double longitude, double latitude, long timeOffset)>();
            var result1 = track.Run(start, farthest, tolerance);
            var result2 = track.Run(farthest, end, tolerance);
            result.AddRange(result1.Take(result1.Count - 1));
            result.AddRange(result2);
            return result;
        }
    }
}