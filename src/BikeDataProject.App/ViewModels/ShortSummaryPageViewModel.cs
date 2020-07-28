using BikeDataProject.App.API;
using BikeDataProject.App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BikeDataProject.App.ViewModels
{
    public class ShortSummaryPageViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command SendTrackCommand { private set; get; }
        public Command DiscardTrackCommand { private set; get; }

        APIHandler handler;

        public ShortSummaryPageViewModel(double dist, TimeSpan time)
        {
            IsEnabledDiscard = true;
            IsEnabledSubmit = true;

            Distance = dist;
            ElapsedTime = time;

            handler = new APIHandler();

            SendTrackCommand = new Command(async () =>
            {
                IsEnabledSubmit = false;
                Guid userId;

                // Check if user has internet connectivity
                var connectivity = Connectivity.NetworkAccess;

                if (connectivity == NetworkAccess.Internet)
                {
                    Running = true;

                    // Check if the user already has a userId stored in the local database
                    // If true => use the already existing userId
                    // If false => request a userId from the server
                    List<UserInfo> userInfo = await App.Database.GetUserInfos();
                    if (userInfo.Count == 0)
                    {
                        UserInfo user = await handler.GetUserId(new UserInfo { Imei = "random imei number" });
                        await App.Database.SaveUserInfo(user);
                        userId = user.UserIdentifier;
                    }
                    else
                    {
                        userId = userInfo[0].UserIdentifier;
                    }

                    // Send the data (all the locations) to the server
                    var result = await SendTracks(userId);

                    // Delete all the locations if the data is succesfully send to the server
                    if (result)
                    {
                        await DeleteAllLocationsAsync();
                    }

                    // Execute code so that the statistics in MainPageViewModel are updated
                    MessagingCenter.Send<ShortSummaryPageViewModel>(this, "reload");

                    Running = false;
                    await NavigateToMainPage();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", "Please enable internet connection", "Ok");
                    IsEnabledSubmit = true;
                }

            });

            DiscardTrackCommand = new Command(async () =>
            {
                IsEnabledDiscard = false;
                bool discard = await DiscardData();

                if (discard)
                {
                    // delete data
                    await DeleteLastRide();
                    await NavigateToMainPage();
                }
                IsEnabledDiscard = true;
            });
        }

        /// <summary>
        /// The total distance of the cycled bike ride (in kilometers)
        /// </summary>
        double distance;
        public double Distance
        {
            get => distance;
            set
            {
                distance = value;
                var args = new PropertyChangedEventArgs(nameof(Distance));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// The total time of the cycled bike ride
        /// </summary>
        TimeSpan elapsedTime;
        public TimeSpan ElapsedTime
        {
            get => elapsedTime;
            set
            {
                elapsedTime = value;
                var args = new PropertyChangedEventArgs(nameof(ElapsedTime));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// To enable/disable ActivityIndicator (spinning wheel)
        /// </summary>
        bool running;
        public bool Running
        {
            get => running;
            set
            {
                running = value;
                var args = new PropertyChangedEventArgs(nameof(Running));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// To enable/disable button
        /// </summary>
        bool isEnabledDiscard;
        public bool IsEnabledDiscard
        {
            get => isEnabledDiscard;
            set
            {
                isEnabledDiscard = value;
                var args = new PropertyChangedEventArgs(nameof(IsEnabledDiscard));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// To enable/disable button
        /// </summary>
        bool isEnabledSubmit;
        public bool IsEnabledSubmit
        {
            get => isEnabledSubmit;
            set
            {
                isEnabledSubmit = value;
                var args = new PropertyChangedEventArgs(nameof(IsEnabledSubmit));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Send all the locations and userId to the server
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<bool> SendTracks(Guid userId)
        {
            List<LocPost> locations = MapLocationsForApi(await GetLocationsAsync());
            return await handler.SendTracks(new Track()
            {
                Locations = locations,
                UserId = userId
            });
        }

        /// <summary>
        /// Get all the locations
        /// </summary>
        /// <returns>A list containing all the locations</returns>
        private async Task<List<Loc>> GetLocationsAsync()
        {
            return await App.Database.GetLocationsAsync();
        }

        /// <summary>
        /// Ask if user really wants to discard the data
        /// </summary>
        /// <returns>True if the users presses 'Yes'; false is the user presses 'No'</returns>
        private async Task<bool> DiscardData()
        {
            return await Application.Current.MainPage.DisplayAlert("Discard data?", "Are you sure you want to discard the data you just collected?", "Yes", "No");
        }

        /// <summary>
        /// Navigates to the root page (MainPage)
        /// </summary>
        private async Task NavigateToMainPage()
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Maps the list of Loc-objects to a list of LocPost objects.
        /// Loc-objects are used to internally store the data, LocPost objects are used to send the data to the server
        /// </summary>
        /// <param name="locations">List of loc objects</param>
        /// <returns>Returns a list of LocPost objects</returns>
        private List<LocPost> MapLocationsForApi(List<Loc> locations)
        {
            List<LocPost> locPosts = new List<LocPost>();
            foreach (Loc location in locations)
            {
                locPosts.Add(new LocPost
                {
                    Longitude = location.Longitude,
                    Latitude = location.Latitude,
                    Altitude = location.Altitude,
                    DateTimeOffset = location.DateTimeOffset,
                    IsFromMockProvider = location.IsFromMockProvider
                });
            }

            return locPosts;
        }

        /// <summary>
        /// Delete all the locations from the last ride and delete the info of the last ride (distance and elapsed time)
        /// </summary>
        /// <returns></returns>
        private async Task DeleteLastRide()
        {
            var lastRide = await App.Database.GetLastRideInfo();
            await App.Database.DeleteLocationsFromRide(lastRide[0].ID);

            await App.Database.DeleteRideAsync(lastRide[0].ID);
        }

        /// <summary>
        /// Delete all the locations
        /// </summary>
        /// <returns></returns>
        private async Task DeleteAllLocationsAsync()
        {
            await App.Database.DeleteAllLocationsAsync();
        }
    }
}