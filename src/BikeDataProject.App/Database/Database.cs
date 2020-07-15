using BikeDataProject.App.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BikeDataProject.App.Database
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            //_database.CreateTableAsync<TrackingOutput>().Wait();
        }

        //public Task<List<TrackingOutput>> GetLocationAsync()
        //{

        //}
    }
}
