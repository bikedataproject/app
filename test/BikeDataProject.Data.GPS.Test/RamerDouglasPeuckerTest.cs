using System.Collections.Generic;
using Xunit;

namespace BikeDataProject.Data.GPS.Test
{
    public class RamerDouglasPeuckerTest
    {
        [Fact]
        public void RamerDouglasPeucker_Run_Empty_ShouldReturnEmpty()
        {
            var empty = new List<(double longitude, double latitude, long timeOffset)>();
            var result = empty.Run();
            Assert.Empty(result);
        }
        
        [Fact]
        public void RamerDouglasPeucker_Run_Single_ShouldReturnSingle()
        {
            var single = new List<(double longitude, double latitude, long timeOffset)>()
            {
                (4.89990234375, 50.590211939351896, 10114)
            };
            var result = single.Run();
            Assert.Single(result);
        }
        
        [Fact]
        public void RamerDouglasPeucker_Run_Pair_ShouldReturnPair()
        {
            var pair = new List<(double longitude, double latitude, long timeOffset)>()
            {
                (4.89990234375, 50.590211939351896, 10114),
                (4.919042587280273, 50.59141071919419, 10254)
            };
            var result = pair.Run();
            Assert.Equal(2, result.Count);
        }
        
        [Fact]
        public void RamerDouglasPeucker_Run_StraightLine_ShouldReturnPair()
        {
            var pair = new List<(double longitude, double latitude, long timeOffset)>
            {
                (
                    4.89840030670166,
                    50.58784153481484, 0),
                (
                    4.905019998550415,
                    50.58794370988537,0),
                (
                    4.91163969039917,
                    50.58804588473417,0),
                (
                    4.918259382247925,
                    50.58814805936125,0),
                (
                    4.92487907409668,
                    50.58825023376658,0),
            };
            var result = pair.Run();
            Assert.Equal(2, result.Count);
        }
        
        [Fact]
        public void RamerDouglasPeucker_Run_Line1_ShouldReturnTriple()
        {
            var track = new List<(double longitude, double latitude, long timeOffset)>
            {
                (
                    4.89840030670166,
                    50.58784153481484, 0),
                (
                    4.905019998550415,
                    50.58794370988537,0),
                (
                    4.911360740661621,
                    50.59187387595808,0),
                (
                    4.918259382247925,
                    50.58814805936125,0),
                (
                    4.92487907409668,
                    50.58825023376658,0),
            };
            
            var result = track.Run();
            Assert.Equal(3, result.Count);
            Assert.Equal(0, (4.89840030670166, 50.58784153481484).DistanceEstimateInMeter(
                (result[0].longitude, result[0].latitude)), 0);
            Assert.Equal(0, (4.911360740661621, 50.5918738759580).DistanceEstimateInMeter(
                (result[1].longitude, result[1].latitude)), 0);
            Assert.Equal(0, (4.92487907409668, 50.58825023376658).DistanceEstimateInMeter(
                (result[2].longitude, result[2].latitude)), 0);
        }
    }
}
