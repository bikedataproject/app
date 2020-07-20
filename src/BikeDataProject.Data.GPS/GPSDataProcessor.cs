using System;
using System.Collections.Generic;

namespace BikeDataProject.Data.GPS
{
    public static class GPSDataProcessor 
    {
        public static (double distance, long time) Process(
            this IReadOnlyList<(double longitude, double latitude, double accuracy, long timeOffset)> track,
            int minLocations = 5, int windowSize = 3)
        {
            var filtered = track.Filter(minLocations, windowSize);

            var distance = 0.0;
            var time = System.Math.Abs(filtered[0].timeOffset - filtered[filtered.Count - 1].timeOffset);
            for (var i = 1; i < filtered.Count; i++)
            {
                var previous = filtered[i - 1];
                var current = filtered[i];
                distance += (previous.longitude, previous.latitude).DistanceEstimateInMeter((current.longitude, current.latitude));
            }

            return (distance, time);
        }

        internal static IReadOnlyList<(double longitude, double latitude, long timeOffset)> Filter(
            this IReadOnlyList<(double longitude, double latitude, double accuracy, long timeOffset)> track,
            int minLocations = 5, int windowSize = 3, double toleranceInMeter = 10)
        {
            if (track.Count < windowSize) return new List<(double longitude, double latitude, long timeOffset)>(0);

            // calculate a running average.
            var runningAverage = track.RunningAverage(windowSize);
            if (runningAverage.Count < minLocations) return new List<(double longitude, double latitude, long timeOffset)>(0);
            
            // execute Ramer–Douglas–Peucker algorithm
            // https://en.wikipedia.org/wiki/Ramer%E2%80%93Douglas%E2%80%93Peucker_algorithm
            return runningAverage.Map(x => (x.longitude, x.latitude, x.timeOffset)).Run(toleranceInMeter);
        }

        internal static IReadOnlyList<(double longitude, double latitude, double accuracy, long timeOffset)>
            RunningAverage(
                this IReadOnlyList<(double longitude, double latitude, double accuracy, long timeOffset)> track, int windowSize = 3)
        {
            return new RunningAverage(track, windowSize);
        }
    }
}
