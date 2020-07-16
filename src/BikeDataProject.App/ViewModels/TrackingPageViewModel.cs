using BikeDataProject.App.API;
using BikeDataProject.App.Models;
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

        APIHandler handler;
        Stopwatch stopwatch;

        private readonly Random _random;

        public TrackingPageViewModel()
        {
            handler = new APIHandler();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            continueTimer = true;
            Distance = 0;
            lastLoc = null;
            _random = new Random();

            Task.Factory.StartNew(async () =>
            {
                rideInfoId = await CreateRideInfoAsync();
            });

            StopTrackingCommand = new Command(async () =>
            {
                continueTimer = false;

                await App.Database.SaveRideInfoAsync(new RideInfo() {ID=rideInfoId, AmountOfKm = Distance, ElapsedTime = ElapsedTime });

                await GetRideInfoAsync();

                //await SendTracks();

                await NavigateToMainPage();
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () => {

                ElapsedTime = stopwatch.Elapsed;
                return continueTimer;
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
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

                        await App.Database.SaveLocationAsync(loc);

                        if (lastLoc != null)
                        {
                            Distance += Location.CalculateDistance(lastLoc.Latitude, lastLoc.Longitude, loc.Latitude, loc.Longitude, DistanceUnits.Kilometers);
                            //Debug.WriteLine($"---------- Distance: {Distance} Accuray: {location.Accuracy}");


                        }
                        lastLoc = loc;

                        //Console.WriteLine($"Accuracy: {location.Accuracy}, Time: {location.Timestamp}, Long: {location.Longitude}, lat: {location.Latitude}");
                    }
                    else
                    {
                        //await SendTracks();
                        await MainThread.InvokeOnMainThreadAsync(this.NavigateToMainPage);
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

        private async Task<bool> SendTracks()
        {
            return await handler.SendTracks(new Track()
            {
                Locations = await GetLocationsAsync(),
                UserId = Guid.Empty
            });
        }


        private async Task NavigateToMainPage()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
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

        private async Task<List<Loc>> GetLocationsAsync()
        {
            var locations = await App.Database.GetLocationsAsync();

            foreach(Loc loc in locations)
            {
                Debug.WriteLine($"---------------- From Database: {loc.ID} {loc.Longitude} {loc.DateTimeOffset} {loc.RideInfoID}");
            }


            return locations;
        }

        private async Task<List<RideInfo>> GetRideInfoAsync()
        {
            var rideInfos = await App.Database.GetRideInfoAsync();

            foreach (RideInfo rideInfo in rideInfos)
            {
                Debug.WriteLine($"---------------- From Database: {rideInfo.ID} {rideInfo.AmountOfKm} {rideInfo.ElapsedTime.TotalSeconds}");
            }


            return rideInfos;
        }

        private async Task<List<Loc>> GetLocationsAsyncByRideInfo()
        {
            var locations = await App.Database.GetLocationsAsync(rideInfoId);
            return locations;
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
