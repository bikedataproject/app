using System;
using System.Collections.Generic;
using System.Text;

namespace BikeDataProject.App.Models
{
    public class TrackingOutput
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public bool IsFromMockProvider { get; set; }
    }
}
