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

        public Command BikeTypeCommand { private set; get; }

        public TrackingSummaryPageViewModel(double dist, TimeSpan time)
        {
            Distance = dist;
            ElapsedTime = time;

            GenderCommand = new Command(option =>
            {
                GenderNotShare = false;
                GenderFemale = false;
                GenderMale = false;
                GenderOther = false;

                switch ($"{option}") {
                    case "Rather not share":
                        GenderNotShare = true;
                        break;
                    case "Female":
                        GenderFemale = true;
                        break;
                    case "Male":
                        GenderMale = true;
                        break;
                    case "Other":
                        GenderOther = true;
                        break;
                }
            });

            AgeRangeCommand = new Command(option =>
            {
                AgeNotShare = false;
                Min18 = false;
                Age18to24 = false;
                Age25to35 = false;
                Age36to45 = false;
                Age46to55 = false;
                Age56to65 = false;
                Plus65 = false;

                switch ($"{option}")
                {
                    case "Rather not share":
                        AgeNotShare = true;
                        break;
                    case "-18":
                        Min18 = true;
                        break;
                    case "18-24":
                        Age18to24 = true;
                        break;
                    case "25-35":
                        Age25to35 = true;
                        break;
                    case "36-45":
                        Age36to45 = true;
                        break;
                    case "46-55":
                        Age46to55 = true;
                        break;
                    case "56-65":
                        Age56to65 = true;
                        break;
                    case "65+":
                        Plus65 = true;
                        break;
                }
            });

            BikeTypeCommand = new Command(option =>
            {
                BikeNotShare = false;
                BikeCity = false;
                BikeMTB = false;
                BikeRace = false;
                BikeElectric = false;
                BikeOther = false;

                switch ($"{option}")
                {
                    case "Rather not share":
                        BikeNotShare = true;
                        break;
                    case "City Bike":
                        BikeCity = true;
                        break;
                    case "MTB":
                        BikeMTB = true;
                        break;
                    case "Race Bike":
                        BikeRace = true;
                        break;
                    case "Electric":
                        BikeElectric = true;
                        break;
                    case "Other":
                        BikeOther = true;
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

        // BikeType
        bool bikeNotShare;
        public bool BikeNotShare
        {
            get => bikeNotShare;
            set
            {
                bikeNotShare = value;
                var args = new PropertyChangedEventArgs(nameof(BikeNotShare));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool bikeCity;
        public bool BikeCity
        {
            get => bikeCity;
            set
            {
                bikeCity = value;
                var args = new PropertyChangedEventArgs(nameof(BikeCity));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool bikeMTB;
        public bool BikeMTB
        {
            get => bikeMTB;
            set
            {
                bikeMTB = value;
                var args = new PropertyChangedEventArgs(nameof(BikeMTB));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool bikeRace;
        public bool BikeRace
        {
            get => bikeRace;
            set
            {
                bikeRace = value;
                var args = new PropertyChangedEventArgs(nameof(BikeRace));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool bikeElectric;
        public bool BikeElectric
        {
            get => bikeElectric;
            set
            {
                bikeElectric = value;
                var args = new PropertyChangedEventArgs(nameof(BikeElectric));
                PropertyChanged?.Invoke(this, args);
            }
        }

        bool bikeOther;
        public bool BikeOther
        {
            get => bikeOther;
            set
            {
                bikeOther = value;
                var args = new PropertyChangedEventArgs(nameof(BikeOther));
                PropertyChanged?.Invoke(this, args);
            }
        }

    }
}
