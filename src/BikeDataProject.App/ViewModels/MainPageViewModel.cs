using BikeDataProject.App.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace BikeDataProject.App.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            StartTrackingCommand = new Command(async () =>
            {
                var trackingVM = new TrackingPageViewModel();
                var trackingPage = new TrackingPage();
                trackingPage.BindingContext = trackingVM;

                await Application.Current.MainPage.Navigation.PushAsync(trackingPage);

            });
        }

        public Command StartTrackingCommand { get; }
    }
}
