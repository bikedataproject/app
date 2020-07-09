using System.ComponentModel;
using Xamarin.Forms;

namespace BikeDataProject.App.ViewModels
{
    public class TrackingPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TrackingPageViewModel()
        {
            StopTrackingCommand = new Command(async () => {
                await Application.Current.MainPage.Navigation.PopAsync();
            });
        }

        public Command StopTrackingCommand { private set; get; }
    }
}
