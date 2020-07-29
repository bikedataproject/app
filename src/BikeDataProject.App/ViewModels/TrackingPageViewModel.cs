﻿using BikeDataProject.App.Models;
using BikeDataProject.App.Views;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BikeDataProject.App.ViewModels
{
    public class TrackingPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Stopwatch to time the current bike ride
        /// </summary>
        Stopwatch stopwatch;

        public TrackingPageViewModel()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            continueTimer = true;
            Distance = 0;
            lastLoc = null;

            IsEnabled = true;

            Task.Factory.StartNew(async () =>
            {
                var rides = await App.Database.GetRideInfoAsync();
                Debug.WriteLine("------------- RideInfo");
                foreach (RideInfo r in rides)
                {
                    Debug.WriteLine($"------------ Id: {r.ID}, dist: {r.AmountOfKm}, time: {r.ElapsedTime}");
                }
                Debug.WriteLine("------------- Locations");
                var locs = await App.Database.GetLocationsAsync();
                foreach (Loc l in locs)
                {
                    Debug.WriteLine($"--------- Loc: {l.ID}, time: {l.DateTimeOffset}, rideid: {l.RideInfoID}");
                }


                rideInfoId = await CreateRideInfoAsync();
                Debug.WriteLine($"----- new ride info object created with id: {rideInfoId}");
            });

            // Save info to database and navigate to TrackingSummaryPage when the stop tracking button is pressed
            StopTrackingCommand = new Command(async () =>
            {
                IsEnabled = false;
                continueTimer = false;
                Running = true;

                await SaveRideInfoAsync();

                IsEnabled = false;
                await NavigateToShortSummaryPage();
                //await NavigateToTrackingSummaryPage();

                Running = false;

            });

            // To get the total time of the current bike ride
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    ElapsedTime = stopwatch.Elapsed;
                });

                return continueTimer;
            });

            // To get the current location and calculate the distance between this location and the previous location
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {

                Task.Factory.StartNew(async () =>
                {
                    var location = await MainThread.InvokeOnMainThreadAsync<Location>(this.GetLocation);
                    if (location != null)
                    {
                        var loc = new Loc { Longitude = location.Longitude, Latitude = location.Latitude, DateTimeOffset = location.Timestamp, RideInfoID = rideInfoId };

                        if (location.Altitude != null)
                        {
                            loc.Altitude = (double)location.Altitude;
                        }
                        if (location.Accuracy != null)
                        {
                            loc.Accuracy = (double)location.Accuracy;
                        }

                        await App.Database.SaveLocationAsync(loc);

                        if (lastLoc != null)
                        {
                            Distance += Location.CalculateDistance(lastLoc.Latitude, lastLoc.Longitude, loc.Latitude, loc.Longitude, DistanceUnits.Kilometers);
                        }
                        lastLoc = loc;
                    }
                    else
                    {
                        if (IsEnabled)
                        {
                            Running = true;
                            await SaveRideInfoAsync();
                            await MainThread.InvokeOnMainThreadAsync(this.NavigateToShortSummaryPage);
                            //await MainThread.InvokeOnMainThreadAsync(this.NavigateToTrackingSummaryPage);
                            Running = false;
                            IsEnabled = false;
                        }
                    }
                });

                return continueTimer;
            });
        }

        public Command StopTrackingCommand { private set; get; }

        /// <summary>
        /// Value to check if Device.StartTimer needs to continue or stop
        /// </summary>
        private bool continueTimer;

        /// <summary>
        /// The Id of the current bike ride
        /// </summary>
        private long rideInfoId;

        /// <summary>
        /// To store the previous location
        /// </summary>
        private Loc lastLoc;

        /// <summary>
        /// The total distance of the current bike ride (in kilometers)
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
        /// The total time of the current bike ride
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
        bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                var args = new PropertyChangedEventArgs(nameof(IsEnabled));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Navigates to the TrackingSummaryPage and initialize the bindingContext to set the connection between the view and the ViewModel
        /// Also add Distance and Elapsed time as parameters to the constructor
        /// </summary>
        private async Task NavigateToTrackingSummaryPage()
        {
            var trackingSummaryVM = new TrackingSummaryPageViewModel(Distance, ElapsedTime);
            var trackingSummaryPage = new TrackingSummaryPage();
            trackingSummaryPage.BindingContext = trackingSummaryVM;

            await Application.Current.MainPage.Navigation.PushAsync(trackingSummaryPage);
        }

        private async Task NavigateToShortSummaryPage()
        {
            var shortSummaryVM = new ShortSummaryPageViewModel(Distance, ElapsedTime);
            var shortSummaryPage = new ShortSummaryPage();
            shortSummaryPage.BindingContext = shortSummaryVM;
            await Application.Current.MainPage.Navigation.PushAsync(shortSummaryPage);
        }

        /// <summary>
        /// Gets the current location of the user
        /// If the feature isn't enabled => set continueTimer to false to stop Device.StartTimer()
        /// </summary>
        /// <returns>Returns the location if the location is enabled, otherwise it returns null</returns>
        private async Task<Location> GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);
                return location;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception (when location isn't turned on)
                Debug.WriteLine($"FeatureNotEnabledException: {fneEx.Message}");
                continueTimer = false;
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something is wrong: {ex.Message}");
                continueTimer = false;
                return null;
            }
        }

        /// <summary>
        /// Create a new RideInfo object and save to database
        /// </summary>
        /// <returns>The id of the created RideInfo object, if something went wrong: return -1</returns>
        private async Task<long> CreateRideInfoAsync()
        {
            await App.Database.SaveRideInfoAsync(new RideInfo() { AmountOfKm = 0, ElapsedTime = new TimeSpan(0) });
            var lastRideInfo = await App.Database.GetLastRideInfo();
            return lastRideInfo.ID;
        }

        /// <summary>
        /// Save the Distance and ElapsedTime to the database in a rideInfo object
        /// </summary>
        private async Task SaveRideInfoAsync()
        {
            Debug.WriteLine($"----------- Save ride info: id: {rideInfoId}");
            await App.Database.SaveRideInfoAsync(new RideInfo() { ID = rideInfoId, AmountOfKm = Distance, ElapsedTime = ElapsedTime });
        }
    }
}