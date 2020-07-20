using System;
using System.Collections.Generic;
using Xunit;

namespace BikeDataProject.Data.GPS.Test
{
    public class RunningAverageTests
    {
        [Fact]
        public void RunningAverage_Empty_ThrowsArgumentOutOfRange()
        {
            var empty = new List<(double longitude, double latitude, double accuracy, long timeOffset)>();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                empty.RunningAverage(3);
            });
        }

        [Fact]
        public void RunningAverage_Window_Returns1()
        {
            var pair = new List<(double longitude, double latitude, double accuracy, long timeOffset)>
            {
                (
                    4.89840030670166,
                    50.58784153481484, 4, 1),
                (
                    4.905019998550415,
                    50.58794370988537, 3, 2),
                (
                    4.91163969039917,
                    50.58804588473417, 5, 3)
            };

            var avg = pair.RunningAverage(3);
            Assert.Single(avg);
            var val = avg[0];
            Assert.Equal(2, val.timeOffset);
            Assert.Equal(4, val.accuracy);
            Assert.Equal(0, (4.905019998550415,
                50.5879437098853).DistanceEstimateInMeter((val.longitude, val.latitude)), 0);
        }
    }
}