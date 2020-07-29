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
            var initRideInfo = false;

            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Loc).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Loc)).ConfigureAwait(false);
                    initLoc = true;
                }

                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(RideInfo).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(RideInfo)).ConfigureAwait(false);
                    initRideInfo = true;
                }
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(UserInfo).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(UserInfo)).ConfigureAwait(false);
                    //if (initLoc && initRideInfo)
                    //{
                    //    initialized = true;
                    //}
                }
                initialized = true;
            }
        }

        /// <summary>
        /// Gets all the locations
        /// </summary>
        /// <returns>A list of all the locations</returns>
        public Task<List<Loc>> GetLocationsAsync()
        {
            return Database.Table<Loc>().ToListAsync();
        }

        /// <summary>
        /// Gets all the locations from a certain bike ride
        /// </summary>
        /// <param name="rideInfoId">The id of the bike ride</param>
        /// <returns>A list of all the locations from a certain bike ride</returns>
        public Task<List<Loc>> GetLocationsAsync(long rideInfoId)
        {
            return Database.QueryAsync<Loc>($"SELECT * FROM [LOC] WHERE [RIDEINFOID] = {rideInfoId}");
        }

        /// <summary>
        /// Saves the location
        /// </summary>
        /// <param name="location">The location to save</param>
        /// <returns>The amount of rows that are added or updated</returns>
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

        /// <summary>
        /// Gets the last bike ride
        /// </summary>
        /// <returns>A list containing the last bike ride</returns>
        public Task<RideInfo> GetLastRideInfo() 
        {
            return Database.Table<RideInfo>().OrderByDescending(r => r.ID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Saves the provided bike ride
        /// </summary>
        /// <param name="rideInfo">The bike ride to save</param>
        /// <returns>The amount of rows that are added or updated</returns>
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

        /// <summary>
        /// Saves the user info
        /// </summary>
        /// <param name="userInfo">The user info to save</param>
        /// <returns>The amount of rows that are added or updated</returns>
        public Task<int> SaveUserInfo(UserInfo userInfo)
        {
            return Database.InsertAsync(userInfo);
        }

        /// <summary>
        /// Gets all the rideInfo's
        /// </summary>
        /// <returns>Returns a list of all the bike rides</returns>
        public Task<List<RideInfo>> GetRideInfoAsync()
        {
            return Database.Table<RideInfo>().ToListAsync();
        }

        /// <summary>
        /// Deletes all the locations from a certain bike ride
        /// </summary>
        /// <param name="rideInfoId">The id of the bike ride</param>
        /// <returns></returns>
        public Task<List<Loc>> DeleteLocationsFromRide(long rideInfoId)
        {
            return Database.QueryAsync<Loc>($"DELETE FROM LOC WHERE RIDEINFOID  = {rideInfoId}");
        }

        /// <summary>
        /// Deletes the bike ride with a certain id
        /// </summary>
        /// <param name="rideInfoId">The id of rideInfo the delete</param>
        /// <returns></returns>
        public Task<List<RideInfo>> DeleteRideAsync(long rideInfoId)
        {
            return Database.QueryAsync<RideInfo>($"DELETE FROM RIDEINFO WHERE ID = {rideInfoId}");
        }

        /// <summary>
        /// Deletes all the locations
        /// </summary>
        /// <returns>The amount of rows that are deleted</returns>
        public Task<int> DeleteAllLocationsAsync()
        {
            return Database.DeleteAllAsync<Loc>();
        }

        /// <summary>
        /// Get the user info
        /// </summary>
        /// <returns>A list of user info objects (the list should maximum have 1 userInfo object)</returns>
        public Task<List<UserInfo>> GetUserInfos()
        {
            return Database.Table<UserInfo>().ToListAsync();
        }

        //public Task<int> DeleteAllRides()
        //{
        //    return Database.DeleteAllAsync<RideInfo>();
        //}

        //public Task<int> DeleteAllUsers()
        //{
        //    return Database.DeleteAllAsync<UserInfo>();
        //}
    }
}
