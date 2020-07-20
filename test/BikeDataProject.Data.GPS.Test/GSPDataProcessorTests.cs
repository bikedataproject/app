using System.Collections.Generic;
using Xunit;

namespace BikeDataProject.Data.GPS.Test
{
    public class GSPDataProcessorTests
    {
        [Fact]
        public void GPSDataProcessor_Filter_Empty_ShouldBeEmpty()
        {
            var track = new List<(double longitude, double latitude, double accuracy, long timeOffset)>();

            var result = track.Filter(5, 3, 10);
            Assert.Empty(result);
        }
        
        [Fact]
        public void GPSDataProcessor_Filter_LessThanMinLocations_ShouldBeEmpty()
        {
            var track = new List<(double longitude, double latitude, double accuracy, long timeOffset)>
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

            var result = track.Filter(5, 3, 10);
            Assert.Empty(result);
        }
        
        [Fact]
        public void GPSDataProcessor_Filter_TwoAfterWindow_ShouldBePair()
        {
            var track = new List<(double longitude, double latitude, double accuracy, long timeOffset)>
            {
                (
                    4.89840030670166,
                    50.58784153481484, 4, 1),
                (
                    4.905019998550415,
                    50.58794370988537, 3, 2),
                (
                    4.905019998550415,
                    50.58794370988537, 3, 2),
                (
                    4.91163969039917,
                    50.58804588473417, 5, 3)
            };

            var result = track.Filter(4, 3, 10);
            Assert.Equal(2, result.Count);
        }
        
        [Fact]
        public void GPSDataProcessor_Filter_TestTrack1_ShouldBePair()
        {
            var track = new List<(double longitude, double latitude, double accuracy, long timeOffset)>
            {
                (
                    
                    4.909618981182575,
                    50.593928653449886, 4, 1),
                (
                    4.90962166339159,
                    50.59395845068209, 3, 2),
                (
                    4.909675642848015,
                    50.59392524805073, 3, 2),
                (
                    4.909625016152859,
                    50.59392780210013, 5, 3),
                (
                    4.909639768302441,
                    50.59394419058047, 5, 3),
                (
                    4.9090661108493805,
                    50.5932760893352, 5, 3),
                (
                    4.909043312072754,
                    50.593325468279936, 5, 3),
                (
                    4.909138530492783,
                    50.59329481928585, 5, 3),
                (
                    4.909032583236694,
                    50.593240332135984, 5, 3),
                (
                    4.909029901027679,
                    50.5932922652021, 5, 3)
            };

            var result = track.Filter(4, 3, 10);
            Assert.Equal(2, result.Count);
        }
        
        [Fact]
        public void GPSDataProcessor_DistanceAndTime_TestTrack1_ShouldBe()
        {
            var track = new List<(double longitude, double latitude, double accuracy, long timeOffset)>
            {
                (
                    
                    4.909618981182575,
                    50.593928653449886, 4, 1),
                (
                    4.90962166339159,
                    50.59395845068209, 3, 2),
                (
                    4.909675642848015,
                    50.59392524805073, 3, 2),
                (
                    4.909625016152859,
                    50.59392780210013, 5, 3),
                (
                    4.909639768302441,
                    50.59394419058047, 5, 3),
                (
                    4.9090661108493805,
                    50.5932760893352, 5, 3),
                (
                    4.909043312072754,
                    50.593325468279936, 5, 3),
                (
                    4.909138530492783,
                    50.59329481928585, 5, 3),
                (
                    4.909032583236694,
                    50.593240332135984, 5, 3),
                (
                    4.909029901027679,
                    50.5932922652021, 5, 3)
            };

            var (distance, time) = track.DistanceAndTime(4, 3, 10);
            Assert.Equal(83.9, distance, 0);
            Assert.Equal(1, time);
        }
    }
}