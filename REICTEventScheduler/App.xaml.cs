using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using REICTEventScheduler.Services;
using REICTEventScheduler.Views;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace REICTEventScheduler
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new LoginPage();
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
