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

            StopTrackingCommand = new Command(async () =>
            {
                continueTimer = false;
                await NavigateToMainPage();
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    var location = await MainThread.InvokeOnMainThreadAsync<Location>(GetLocation);
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
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(NavigateToMainPage);
                    }
                });


                return continueTimer;
            });
        }

        public Command StopTrackingCommand { private set; get; }

        private List<TrackingOutput> GetTrackingOutputs;

        private bool continueTimer;

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
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Debug.WriteLine($"FeatureNotSupportedException: {fnsEx.Message}");
                return null;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception (when location isn't turned on)
                Debug.WriteLine($"FeatureNotEnabledException: {fneEx.Message}");
                continueTimer = false;
                return null;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Debug.WriteLine($"PermissionException: {pEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something is wrong: {ex.Message}");
                return null;
            }
        }
    }
}
