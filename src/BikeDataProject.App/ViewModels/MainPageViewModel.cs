using BikeDataProject.App.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace BikeDataProject.App.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            Running = false;
            StartTrackingCommand = new Command(async () =>
            {
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
            });
        }

        public Command StartTrackingCommand { get; }

        Boolean running;
        public Boolean Running
        {
            get => running;
            set
            {
                running = value;
                var args = new PropertyChangedEventArgs(nameof(Running));
                PropertyChanged?.Invoke(this, args);
            }
        }

        private async Task<Boolean> GetLocationPermissions()
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
    }
}
