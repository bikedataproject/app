using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BikeDataProject.App
{
    public partial class App : Application
    {
        static Database.DatabaseAccess database;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        public static Database.DatabaseAccess Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database.DatabaseAccess();
                }
                return database;
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
