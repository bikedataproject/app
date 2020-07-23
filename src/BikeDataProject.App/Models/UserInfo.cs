using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BikeDataProject.App.Models
{
    public class UserInfo
    {
        public string Imei { get; set; }

        [PrimaryKey]
        public Guid UserIdentifier { get; set; }
    }
}
