using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace BikeDataProject.App.ViewModels
{
    public class TrackingSummaryPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command GenderCommand { private set; get; }

        public TrackingSummaryPageViewModel(double dist, TimeSpan time)
        {
            Distance = dist;
            ElapsedTime = time;

            GenderCommand = new Command((option) =>
            {
                Debug.WriteLine($"The label is pressed: {option}");

                switch ($"{option}") {
                    case "Rather not share":
                        GenderNotShare = true;
                        GenderFemale = false;
                        GenderMale = false;
                        GenderOther = false;
                        break;
                    case "Female":
                        GenderNotShare = false;
                        GenderFemale = true;
                        GenderMale = false;
                        GenderOther = false;
                        break;
                    case "Male":
                        GenderNotShare = false;
                        GenderFemale = false;
                        GenderMale = true;
                        GenderOther = false;
                        break;
                    case "Other":
                        GenderNotShare = false;
                        GenderFemale = false;
                        GenderMale = false;
                        GenderOther = true;
                        break;
                }


            });
        }

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

        // Gender values
        bool genderNotShare;
        public bool GenderNotShare
        {
            get => genderNotShare;
            set
            {
                genderNotShare = value;
                var args = new PropertyChangedEventArgs(nameof(GenderNotShare));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool genderFemale;
        public bool GenderFemale
        {
            get => genderFemale;
            set
            {
                genderFemale = value;
                var args = new PropertyChangedEventArgs(nameof(GenderFemale));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool genderMale;
        public bool GenderMale
        {
            get => genderMale;
            set
            {
                genderMale = value;
                var args = new PropertyChangedEventArgs(nameof(GenderMale));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool genderOther;
        public bool GenderOther
        {
            get => genderOther;
            set
            {
                genderOther = value;
                var args = new PropertyChangedEventArgs(nameof(GenderOther));
                PropertyChanged?.Invoke(this, args);
            }
        }


    }
}
