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
            Running = false;

            _ = InitializeStatisticsAsync();

            StartTrackingCommand = new Command(async () =>
            {
                lock (_sync)
                {

                    if (_state) return;
                    _state = true;
                }

                try
                {
                    //await InitializeStatisticsAsync();
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

        private async Task NavigateToTrackingPage()
        {
            var trackingVM = new TrackingPageViewModel();
            var trackingPage = new TrackingPage();
            trackingPage.BindingContext = trackingVM;

            await Application.Current.MainPage.Navigation.PushAsync(trackingPage);
        }

        private async Task RequestLocationPermission()
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        private async Task InitializeStatisticsAsync() 
        {
            List<RideInfo> rideInfos = await App.Database.GetRideInfoAsync();
            //foreach (RideInfo ride in rideInfos) 
            //{
            //    Debug.WriteLine($"-------- {ride.AmountOfKm} {ride.ElapsedTime} {ride.ID}");
            //}

            TotalDistance = CalculateTotalDistance(rideInfos);
            TotalTime = CalculateTotalTime(rideInfos);

            //Debug.WriteLine($"------------- Total distance: {totalDistance} Total time: {totalTime}");
        }

        private double CalculateTotalDistance(List<RideInfo> rides) 
        {
            return rides.Sum(ride => ride.AmountOfKm);
        }

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
