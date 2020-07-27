using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BikeDataProject.App
{
    public static class Constants
    {
        // API
        public const string BaseAPIUri = "https://api.bikedataproject.info/";
        public const string StoreTrackEndPoint = "geo/Track/StoreTrack";
        public const string UserIdEndPoint = "registrations/MobileApp";
        public const string WorldStatisticsEndPoint = "geo/Track/Publish";

        public const string ApplicationJson = "application/json";

        // Database
        public const string DatabaseFilename = "BikeDataSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }

    }
}
