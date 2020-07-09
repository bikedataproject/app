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

        public TrackingPageViewModel()
        {
            continueTimer = true;
            GetTrackingOutputs = new List<TrackingOutput>();

            StopTrackingCommand = new Command(async () => {
                continueTimer = false;
                await Application.Current.MainPage.Navigation.PopAsync();
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(5000), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    var location = await GetLocation();
                    if (location != null)
                    {

                        var trackingOutput = new TrackingOutput { Longitude = location.Longitude, Latitude = location.Latitude, DateTimeOffset = location.Timestamp };
                        if (location.Altitude != null)
                        {
                            trackingOutput.Altitude = (double)location.Altitude;
                        }


                        GetTrackingOutputs.Add(trackingOutput);
                        Console.WriteLine($"Accuracy: {location.Accuracy}, Time: {location.Timestamp}, Long: {location.Longitude}, lat: {location.Latitude}");
                    }
                });

                return continueTimer;
            });
        }

        public Command StopTrackingCommand { private set; get; }

        private List<TrackingOutput> GetTrackingOutputs;

        private bool continueTimer;

        private async Task<Location> GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);
                return location;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something is wrong: {ex.Message}");
                return null;
            }
        }
    }
}
