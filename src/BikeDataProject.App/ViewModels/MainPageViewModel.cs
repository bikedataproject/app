using BikeDataProject.App.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
            StartTrackingCommand = new Command(async () =>
            {
                if (await GetLocationPermissions())
                {
                    await NavigateToTrackingPage();
                }
                else
                {
                    await RequestLocationPermission();
                    if (await GetLocationPermissions())
                    {
                        await NavigateToTrackingPage();
                    }
                }

            });
        }

        public Command StartTrackingCommand { get; }

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
