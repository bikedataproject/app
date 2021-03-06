namespace BikeDataProject.Data.GPS
{
    internal static class StaticGeo
    {
        const double RadiusOfEarth = 6371000;
        const double E = 0.0000000001;
        
        /// <summary>
        /// Returns an estimate of the distance between the two given coordinates.
        /// </summary>
        /// <param name="coordinate1">The first coordinate.</param>
        /// <param name="coordinate2">The second coordinate.</param>
        /// <remarks>Accuracy decreases with distance.</remarks>
        public static double DistanceEstimateInMeter(this (double longitude, double latitude) coordinate1, 
            (double longitude, double latitude) coordinate2)
        {
            var lat1Rad = (coordinate1.latitude / 180d) * System.Math.PI;
            var lon1Rad = (coordinate1.longitude / 180d) * System.Math.PI;
            var lat2Rad = (coordinate2.latitude / 180d) * System.Math.PI;
            var lon2Rad = (coordinate2.longitude / 180d) * System.Math.PI;

            var x = (lon2Rad - lon1Rad) * System.Math.Cos((lat1Rad + lat2Rad) / 2.0);
            var y = lat2Rad - lat1Rad;

            var m = System.Math.Sqrt(x * x + y * y) * RadiusOfEarth;

            return m;
        }
        
        /// <summary>
        /// Projects for coordinate on this line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns>The project coordinate.</returns>
        public static (double longitude, double latitude)? ProjectOn(this ((double longitude, double latitude) coordinate1, 
            (double longitude, double latitude) coordinate2) line, (double longitude, double latitude) coordinate,
            bool segment = false)
        {
            var coordinate1 = line.coordinate1;
            var coordinate2 = line.coordinate2;
            
            // TODO: do we need to calculate the expensive length in meter, this can be done more easily.
            var lengthInMeters = line.coordinate1.DistanceEstimateInMeter(line.coordinate2);
            if (lengthInMeters < E)
            { 
                return null;
            }

            // get direction vector.
            var diffLat = (coordinate2.latitude - coordinate1.latitude);
            var diffLon = (coordinate2.longitude - coordinate1.longitude);

            // increase this line in length if needed.
            var longerLine = line;
            if (lengthInMeters < 50)
            {
                longerLine = (coordinate1, (diffLon + coordinate.longitude, diffLat + coordinate.latitude));
            }

            // rotate 90°, offset y with x, and x with y.
            var xLength = longerLine.coordinate1.DistanceEstimateInMeter((longerLine.coordinate2.longitude, longerLine.coordinate1.latitude));
            if (longerLine.coordinate1.longitude > longerLine.coordinate2.longitude) xLength = -xLength;
            var yLength = longerLine.coordinate1.DistanceEstimateInMeter((longerLine.coordinate1.longitude, longerLine.coordinate2.latitude));
            if (longerLine.coordinate1.latitude > longerLine.coordinate2.latitude) yLength = -yLength;
            
            var second = coordinate.OffsetWithDistanceY(xLength)
                .OffsetWithDistanceX(-yLength);

            // create a second line.
            var other = (coordinate, second);

            // calculate intersection.
            var projected = longerLine.Intersect(other, false);

            // check if coordinate is on this line.
            if (!projected.HasValue)
            {
                return null;
            }
            
            // check if the coordinate is on this line.
            if (segment)
            {
                var dist = line.A() * line.A() + line.B() * line.B();
                var line1 = (projected.Value, coordinate1);
                var distTo1 = line1.A() * line1.A() + line1.B() * line1.B();
                if (distTo1 > dist)
                {
                    return null;
                }

                var line2 = (projected.Value, coordinate2);
                var distTo2 = line2.A() * line2.A() + line2.B() * line2.B();
                if (distTo2 > dist)
                {
                    return null;
                }
            }

            return projected;
        }
        
        /// <summary>
        /// Calculates the intersection point of the given line with this line. 
        /// 
        /// Returns null if the lines have the same direction or don't intersect.
        /// 
        /// Assumes the given line is not a segment and this line is a segment.
        /// </summary>
        public static (double longitude, double latitude)? Intersect(this ((double longitude, double latitude) coordinate1, 
            (double longitude, double latitude) coordinate2) thisLine, ((double longitude, double latitude) coordinate1, 
            (double longitude, double latitude) coordinate2) line, bool checkSegment = true)
        {
            var det = (double)(line.A() * thisLine.B() - thisLine.A() * line.B());
            if (System.Math.Abs(det) <= E)
            { // lines are parallel; no intersections.
                return null;
            }
            else
            { // lines are not the same and not parallel so they will intersect.
                var x = (thisLine.B() * line.C() - line.B() * thisLine.C()) / det;
                var y = (line.A() * thisLine.C() - thisLine.A() * line.C()) / det;

                var coordinate = (x ,y);

                // check if the coordinate is on this line.
                if (checkSegment)
                {
                    var dist = thisLine.A() * thisLine.A() + thisLine.B() * thisLine.B();
                    var line1 = (coordinate, thisLine.coordinate1);
                    var distTo1 = line1.A() * line1.A() + line1.B() * line1.B();
                    if (distTo1 > dist)
                    {
                        return null;
                    }

                    var line2 = (coordinate, thisLine.coordinate2);
                    var distTo2 = line2.A() * line2.A() + line2.B() * line2.B();
                    if (distTo2 > dist)
                    {
                        return null;
                    }
                }
                
                return coordinate;
            }
        }

        private static double A(this ((double longitude, double latitude) coordinate1,
            (double longitude, double latitude) coordinate2) line)
        {
            return line.coordinate2.latitude - line.coordinate1.latitude;
        }

        private static double B(this ((double longitude, double latitude) coordinate1,
            (double longitude, double latitude) coordinate2) line)
        {
            return line.coordinate1.longitude - line.coordinate2.longitude;
        }

        private static double C(this ((double longitude, double latitude) coordinate1,
            (double longitude, double latitude) coordinate2) line)
        {
            return line.A() * line.coordinate1.longitude +
                   line.B() * line.coordinate1.latitude;
        }
        
        /// <summary>
        /// Returns a coordinate offset with a given distance.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <param name="meter">The distance.</param>
        /// <returns>An offset coordinate.</returns>
        public static (double longitude, double latitude) OffsetWithDistanceX(this (double longitude, double latitude) coordinate, double meter)
        {
            var offset = 0.001;
            var offsetLon = (coordinate.longitude + offset, coordinate.latitude);
            var lonDistance = offsetLon.DistanceEstimateInMeter(coordinate);

            return (coordinate.longitude + (meter / lonDistance) * offset, 
                coordinate.latitude);
        }

        /// <summary>
        /// Returns a coordinate offset with a given distance.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <param name="meter">The distance.</param>
        /// <returns>An offset coordinate.</returns>
        public static (double longitude, double latitude) OffsetWithDistanceY(this (double longitude, double latitude) coordinate, double meter)
        {
            var offset = 0.001;
            var offsetLat = (coordinate.longitude, coordinate.latitude + offset);
            var latDistance = offsetLat.DistanceEstimateInMeter(coordinate);

            return (coordinate.longitude, 
                coordinate.latitude + (meter / latDistance) * offset);
        }
    }
}