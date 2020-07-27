using BikeDataProject.App.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using BikeDataProject.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace BikeDataProject.App.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _state = false;
        private object _sync = new object();

        public MainPageViewModel()
        {
            // Make sure the spinnnig wheel isn't running
            Running = false;

            _ = InitializeStatisticsAsync();

            // Execute this code when the button in MainPage is pressed
            StartTrackingCommand = new Command(async () =>
            {
                // Make sure the user can't press the button multiple times
                lock (_sync)
                {

                    if (_state) return;
                    _state = true;
                }

                try
                {
                    // Navigate to TrackingPage if app has locationpermission and the location is enabled
                    if (await GetLocationPermissions())
                    {
                        if (await GetLocation())
                        {
                            await NavigateToTrackingPage();
                        }
                    }
                    else
                    {
                        await RequestLocationPermission();
                        if (await GetLocationPermissions())
                        {
                            if (await GetLocation())
                            {
                                await NavigateToTrackingPage();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // TODO: log exception here!
                }
                finally
                {
                    _state = false;
                }
            });
        }

        public Command StartTrackingCommand { get; }

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
        /// The total distance that a user has cycled (in meters)
        /// </summary>
        double totalDistance;
        public double TotalDistance
        {
            get => totalDistance;
            set
            {
                totalDistance = value;
                var args = new PropertyChangedEventArgs(nameof(TotalDistance));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// The total time the user has cycled
        /// </summary>
        TimeSpan totalTime;
        public TimeSpan TotalTime
        {
            get => totalTime;
            set
            {
                totalTime = value;
                var args = new PropertyChangedEventArgs(nameof(TotalTime));
                PropertyChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Check if the user has granted permission to use the location
        /// </summary>
        /// <returns>True if the user has granted permissions, else false</returns>
        private async Task<bool> GetLocationPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the current location of the user
        /// This function is just to check if the location is enabled and supported on the phone
        /// </summary>
        /// <returns>True if no exception is thrown</returns>
        private async Task<Boolean> GetLocation()
        {
            try
            {
                Running = true;
                var request = new GeolocationRequest(GeolocationAccuracy.Lowest);
                var location = await Geolocation.GetLocationAsync(request);
                Running = false;
                return true;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Running = false;
                // Handle not supported on device exception
                await Application.Current.MainPage.DisplayAlert("Alert", "The location feature is not supported on your phone.", "OK");
                return false;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Running = false;
                // Handle not enabled on device exception (when location isn't turned on)
                await Application.Current.MainPage.DisplayAlert("Alert", "Please turn on location.", "OK");
                return false;
            }
            catch (Exception ex)
            {
                Running = false;
                await Application.Current.MainPage.DisplayAlert("Alert", "Something went wrong, please try again later.", "OK");
                return false;
            }

        }

        /// <summary>
        /// Navigates to the TrackingPage and and initialize the bindingContext to set the connection between the view and the ViewModel
        /// </summary>
        private async Task NavigateToTrackingPage()
        {
            var trackingVM = new TrackingPageViewModel();
            var trackingPage = new TrackingPage();
            trackingPage.BindingContext = trackingVM;

            await Application.Current.MainPage.Navigation.PushAsync(trackingPage);
        }

        /// <summary>
        /// Request permission to use location functionality
        /// </summary>
        /// <returns></returns>
        private async Task RequestLocationPermission()
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        /// <summary>
        /// Initialize the statistics, these are: TotalDistance and TotalTime
        /// </summary>
        private async Task InitializeStatisticsAsync() 
        {
            List<RideInfo> rideInfos = await App.Database.GetRideInfoAsync();

            TotalDistance = CalculateTotalDistance(rideInfos);
            TotalTime = CalculateTotalTime(rideInfos);
        }

        /// <summary>
        /// Calculate the total distance that a user has cycled
        /// </summary>
        /// <param name="rides">List of RideInfo-objects</param>
        /// <returns>The total cycled distance in meters</returns>
        private double CalculateTotalDistance(List<RideInfo> rides) 
        {
            return rides.Sum(ride => ride.AmountOfKm);
        }

        /// <summary>
        /// Calculate the total time that a user has cycled
        /// </summary>
        /// <param name="rides">List of RideInfo-objects</param>
        /// <returns>The total cylcled time</returns>
        private TimeSpan CalculateTotalTime(List<RideInfo> rides) 
        {
            TimeSpan total = new TimeSpan(0);
            foreach (RideInfo ride in rides) 
            {
                total += ride.ElapsedTime;
            }
            return total;
        }
    }
}
