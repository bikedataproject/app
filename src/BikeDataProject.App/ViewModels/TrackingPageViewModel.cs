using BikeDataProject.App.API;
using BikeDataProject.App.Models;
using BikeDataProject.App.Views;
using System;
using System.Collections.Generic;
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

        private bool _state = false;
        private object _sync = new object();

        Stopwatch stopwatch;

        public TrackingPageViewModel()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            continueTimer = true;
            Distance = 0;
            lastLoc = null;

            Task.Factory.StartNew(async () =>
            {
                rideInfoId = await CreateRideInfoAsync();
            });

            StopTrackingCommand = new Command(async () =>
            {
                //This doesn't work yet
                lock (_sync)
                {

                    if (_state) return;
                    _state = true;
                }

                try
                {
                    continueTimer = false;

                    await App.Database.SaveRideInfoAsync(new RideInfo() { ID = rideInfoId, AmountOfKm = Distance, ElapsedTime = ElapsedTime });

                    await NavigateToTrackingSummaryPage();
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

            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    ElapsedTime = stopwatch.Elapsed;
                });

                return continueTimer;
            });

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
                        await MainThread.InvokeOnMainThreadAsync(this.NavigateToTrackingSummaryPage);
                    }
                });

                return continueTimer;
            });
        }

        public Command StopTrackingCommand { private set; get; }

        private bool continueTimer;

        private long rideInfoId;

        private Loc lastLoc;

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

        private async Task NavigateToTrackingSummaryPage()
        {
            var trackingSummaryVM = new TrackingSummaryPageViewModel(Distance, ElapsedTime);
            var trackingSummaryPage = new TrackingSummaryPage();
            trackingSummaryPage.BindingContext = trackingSummaryVM;

            await Application.Current.MainPage.Navigation.PushAsync(trackingSummaryPage);
        }

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
                return null;
            }
        }

        private async Task<long> CreateRideInfoAsync()
        {
            await App.Database.SaveRideInfoAsync(new RideInfo() { AmountOfKm = 0, ElapsedTime = new TimeSpan(0) });
            var ids = await App.Database.GetLastRideInfoId();

            if (ids.Count > 0) return ids.First().ID;
            return -1;
        }
    }
}
