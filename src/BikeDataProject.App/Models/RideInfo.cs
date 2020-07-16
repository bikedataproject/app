using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BikeDataProject.App.Models
{
    public class RideInfo
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }

        public Double AmountOfKm { get; set; }

        [OneToMany]
        public List<Loc> Locations { get; set; }
    }
}
