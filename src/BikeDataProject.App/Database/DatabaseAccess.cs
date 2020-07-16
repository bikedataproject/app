using BikeDataProject.App.Extensions;
using BikeDataProject.App.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeDataProject.App.Database
{
    public class DatabaseAccess
    {
        // Using lazy initialization prevents the database loading process from delaying the app launch.
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public DatabaseAccess()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        /// <summary>
        /// Check if table already exists and create the table if it doesn't exist
        /// </summary>
        /// <returns></returns>
        async Task InitializeAsync()
        {
            var initLoc = false;
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Loc).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Loc)).ConfigureAwait(false);
                    initLoc = true;
                    //initialized = true;
                }

                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(RideInfo).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(RideInfo)).ConfigureAwait(false);
                    if (initLoc)
                    {
                        initialized = true;
                    }
                    //initialized = true;
                }
            }
        }

        public Task<List<Loc>> GetLocationsAsync()
        {
            return Database.Table<Loc>().ToListAsync();
        }

        public Task<List<Loc>> GetLocationsAsync(long rideInfoId) 
        {
            return Database.QueryAsync<Loc>($"SELECT * FROM [LOC] WHERE [RIDEINFOID] = {rideInfoId}");
        }

        public Task<int> SaveLocationAsync(Loc location)
        {
            if(location.ID != 0)
            {
                return Database.UpdateAsync(location);
            }
            else
            {
                return Database.InsertAsync(location);
            }
        }

        public Task<List<RideInfo>> GetLastRideInfoId()
        {
            return Database.QueryAsync<RideInfo>("SELECT * FROM RIDEINFO ORDER BY ID DESC LIMIT 1");
        }

        public Task<int> SaveRideInfoAsync(RideInfo rideInfo)
        {
            if (rideInfo.ID != 0)
            {
                return Database.UpdateAsync(rideInfo);
            }
            else
            {
                return Database.InsertAsync(rideInfo);
            }
        }

        public Task<List<RideInfo>> GetRideInfoAsync() 
        {
            return Database.Table<RideInfo>().ToListAsync();        
        }



    }
}
