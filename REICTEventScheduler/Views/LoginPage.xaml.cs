using REICTEventScheduler.Models;
using REICTEventScheduler.Services;
using REICTEventScheduler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace REICTEventScheduler.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        readonly LoginViewModel<Person> viewModel;

        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        public LoginPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new LoginViewModel<Person>();

            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            
            {
                DisplayMessage("No internet", "A connection to the internet is not available and is necessary for the application to function. Please connect to the internet.").ConfigureAwait(false);
                ExitApplication();
            }
            
            EnableLoginButton();
            SetSystemVersion();
        }

        private async void SetSystemVersion()
        {
            while (FirebaseDBBase.isBusy) { }
            if (!await CheckSystemVersion().ConfigureAwait(false))
                return;
        }

        private void EnableLoginButton()
        {
            btnLogon.IsEnabled = !string.IsNullOrEmpty(txtCellNumber.Text) && txtCellNumber.Text.Length == 10 && !string.IsNullOrEmpty(txtPassword.Text);
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = (Entry)sender;
            if (!txt.IsPassword && !string.IsNullOrEmpty(txt.Text))
            {
                try
                {
                    int cellNumber = Convert.ToInt32(txt.Text);
                }
                catch (Exception)
                {
                    txt.Text = txt.Text.Substring(0, txt.Text.Length - 1);
                    DisplayMessage("Only numbers", "Please enter only numbers in this field.").ConfigureAwait(false);
                    return;
                }
            }
            EnableLoginButton();
        }

        private async Task<bool> CheckSystemVersion()
        {
            if (viewModel.CheckSystemVersion() == false)
            {
                if (await DisplayMessage("Incorrect system version", "The current version is outdated, please install the latest version.").ConfigureAwait(true))
                {
                    ExitApplication();
                }
                return false;
            }
            else
                return true;
        }

        private async Task<bool> DisplayMessage(string heading, string message)
        {
            await DisplayAlert(heading, message, "Ok").ConfigureAwait(true);
            return true;
        }

        private async void Login(string cellNumber, string password)
        {
            while (FirebaseDBBase.isBusy) { }
            if (!await CheckSystemVersion().ConfigureAwait(false))
                return;

            if (viewModel.Login(cellNumber, password) == false)
            {
                await DisplayMessage("Incorrect cell number or password", "The cell number and password combination is not correct. Please try again...").ConfigureAwait(true);
                txtCellNumber.Focus();
                txtCellNumber.Text = "";
                txtPassword.Text = "";
            }
            else
            {
                Application.Current.MainPage = new MainPage();

                //Device.BeginInvokeOnMainThread(async () =>
                //{
                //    await Navigation.PushModalAsync(new MainPage()).ConfigureAwait(true);
                //});
            }
        }

        private void btnLogon_Clicked(object sender, EventArgs e)
        {
            Login(txtCellNumber.Text, txtPassword.Text);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await DisplayAlert("Alert!", "want to exit?", "Yes", "No").ConfigureAwait(true);
                if (result)
                {
                    ExitApplication();
                }
            });
            return true;
        }

        private static void ExitApplication()
        {
            if (DeviceInfo.Platform.ToString() == Device.Android)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else if (DeviceInfo.Platform.ToString() == Device.iOS)
            {
                Thread.CurrentThread.Abort();
            }
        }
    }
}