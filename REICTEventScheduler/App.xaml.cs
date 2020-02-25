using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using REICTEventScheduler.Services;
using REICTEventScheduler.Views;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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
            AppCenter.Start("android=1cefe26d-3313-4e2c-9ab8-cd4ea434d7ce;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
