using System;
using System.Collections.Generic;
using System.Text;

namespace BikeDataProject.App.Models
{
    public class Track
    {
        public List<LocPost> Locations { get; set; }
        public Guid UserId { get; set; }
    }
}
