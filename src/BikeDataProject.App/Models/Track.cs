using System;
using System.Collections.Generic;
using System.Text;

namespace BikeDataProject.App.Models
{
    public class Track
    {
        public List<Loc> Locations { get; set; }
        public Guid UserId { get; set; }
    }
}
