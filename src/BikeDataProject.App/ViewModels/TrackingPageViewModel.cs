using BikeDataProject.App.API;
using BikeDataProject.App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BikeDataProject.App.ViewModels
{
    public class TrackingPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        APIHandler handler;

        public TrackingPageViewModel()
        {
            handler = new APIHandler();
            continueTimer = true;

            StopTrackingCommand = new Command(async () =>
            {
                continueTimer = false;

                await GetLocationsAsync();

                //await SendTracks();

                await NavigateToMainPage();
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    var location = await MainThread.InvokeOnMainThreadAsync<Location>(this.GetLocation);
                    if (location != null)
                    {

                        var loc = new Loc { Longitude = location.Longitude, Latitude = location.Latitude, DateTimeOffset = location.Timestamp };
                        if (location.Altitude != null)
                        {
                            loc.Altitude = (double)location.Altitude;
                        }

                        await App.Database.SaveLocationAsync(loc);

                        Console.WriteLine($"Accuracy: {location.Accuracy}, Time: {location.Timestamp}, Long: {location.Longitude}, lat: {location.Latitude}");
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
                Debug.WriteLine($"---------------- From Database: {loc.ID} {loc.Longitude} {loc.DateTimeOffset}");
            }

            return locations;
        }
    }
}
