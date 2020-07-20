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

        public Command AgeRangeCommand { private set; get; }

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

            AgeRangeCommand = new Command(option =>
            {
                Debug.WriteLine($"The label is pressed: {option}");
                switch ($"{option}")
                {
                    case "Rather not share":
                        AgeNotShare = true;
                        Min18 = false;
                        Age18to24 = false;
                        Age25to35 = false;
                        Age36to45 = false;
                        Age46to55 = false;
                        Age56to65 = false;
                        Plus65 = false;
                        break;
                    case "-18":
                        AgeNotShare = false;
                        Min18 = true;
                        Age18to24 = false;
                        Age25to35 = false;
                        Age36to45 = false;
                        Age46to55 = false;
                        Age56to65 = false;
                        Plus65 = false;
                        break;
                    case "18-24":
                        AgeNotShare = false;
                        Min18 = false;
                        Age18to24 = true;
                        Age25to35 = false;
                        Age36to45 = false;
                        Age46to55 = false;
                        Age56to65 = false;
                        Plus65 = false;
                        break;
                    case "25-35":
                        AgeNotShare = false;
                        Min18 = false;
                        Age18to24 = false;
                        Age25to35 = true;
                        Age36to45 = false;
                        Age46to55 = false;
                        Age56to65 = false;
                        Plus65 = false;
                        break;
                    case "36-45":
                        AgeNotShare = false;
                        Min18 = false;
                        Age18to24 = false;
                        Age25to35 = false;
                        Age36to45 = true;
                        Age46to55 = false;
                        Age56to65 = false;
                        Plus65 = false;
                        break;
                    case "46-55":
                        AgeNotShare = false;
                        Min18 = false;
                        Age18to24 = false;
                        Age25to35 = false;
                        Age36to45 = false;
                        Age46to55 = true;
                        Age56to65 = false;
                        Plus65 = false;
                        break;
                    case "56-65":
                        AgeNotShare = false;
                        Min18 = false;
                        Age18to24 = false;
                        Age25to35 = false;
                        Age36to45 = false;
                        Age46to55 = false;
                        Age56to65 = true;
                        Plus65 = false;
                        break;
                    case "65+":
                        AgeNotShare = false;
                        Min18 = false;
                        Age18to24 = false;
                        Age25to35 = false;
                        Age36to45 = false;
                        Age46to55 = false;
                        Age56to65 = false;
                        Plus65 = true;
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

        // Gender
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

        // Age Range
        bool ageNotShare;
        public bool AgeNotShare
        {
            get => ageNotShare;
            set
            {
                ageNotShare = value;
                var args = new PropertyChangedEventArgs(nameof(AgeNotShare));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool min18;
        public bool Min18
        {
            get => min18;
            set
            {
                min18 = value;
                var args = new PropertyChangedEventArgs(nameof(Min18));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool age18to24;
        public bool Age18to24
        {
            get => age18to24;
            set
            {
                age18to24 = value;
                var args = new PropertyChangedEventArgs(nameof(Age18to24));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool age25to35;
        public bool Age25to35
        {
            get => age25to35;
            set
            {
                age25to35 = value;
                var args = new PropertyChangedEventArgs(nameof(Age25to35));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool age36to45;
        public bool Age36to45
        {
            get => age36to45;
            set
            {
                age36to45 = value;
                var args = new PropertyChangedEventArgs(nameof(Age36to45));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool age46to55;
        public bool Age46to55
        {
            get => age46to55;
            set
            {
                age46to55 = value;
                var args = new PropertyChangedEventArgs(nameof(Age46to55));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool age56to65;
        public bool Age56to65
        {
            get => age56to65;
            set
            {
                age56to65 = value;
                var args = new PropertyChangedEventArgs(nameof(Age56to65));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool plus65;
        public bool Plus65
        {
            get => plus65;
            set
            {
                plus65 = value;
                var args = new PropertyChangedEventArgs(nameof(Plus65));
                PropertyChanged?.Invoke(this, args);
            }
        }

    }
}
