using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace BikeDataProject.App.ViewModels
{
    public class ShortSummaryPageViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ShortSummaryPageViewModel(double dist, TimeSpan time)
        {
            Debug.WriteLine($"------------- distance: {dist} time: {time}");
        }

    }
}
