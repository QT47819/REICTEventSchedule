using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using REICTEventScheduler.Models;
using REICTEventScheduler.ViewModels;
using REICTEventScheduler.Services;
using Android.Content;

using Xamarin.Essentials;

namespace REICTEventScheduler.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventDetailPage : ContentPage
    {
        EventDetailViewModel viewModel;
        static Countdown countdown;
        static TimeSpan timeSpan;

        public EventDetailPage(EventDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            if (viewModel.Event.IsVisible == false)
                button.Text = "Reject Attendance";
            else
                button.Text = "Confirm Attendance";

            if (viewModel.Event.AlarmSet)
                btnAlarm.IsVisible = false;

            SetCountdown();
        }

        private void SetCountdown()
        {
            timeSpan = viewModel.Event.Time - DateTime.Now;

            countdown = new Countdown();
            countdown.StartUpdating(timeSpan.TotalSeconds);

            lblTime.SetBinding(Label.TextProperty,
                new Binding("RemainTime", BindingMode.Default, new CountdownConverter()));

            lblTime.BindingContext = countdown;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            var vm = (EventDetailViewModel)btn.CommandParameter;
            Event @event = vm.Event;

            try
            {
                IsBusy = true;

                if (viewModel.Event.IsVisible == true)
                {
                    button.Text = "Reject Attendance";
                    @event.IsVisible = false;

                    if (@event.FirstRespondent == null)
                    {
                        @event.FirstRespondent = Global.LoggedInPerson;
                        lblFirstRespondent.Text = String.Format("{0} {1} {2}", Global.LoggedInPerson.Role.Name, Global.LoggedInPerson.Name, Global.LoggedInPerson.Surname);

                        //lblEventName.BackgroundColor = Color.Orange;
                        @event.Colour = "Orange";
                    }
                    else if (@event.SecondRespondent == null)
                    {
                        @event.SecondRespondent = Global.LoggedInPerson;
                        lblSecondRespondent.Text = String.Format("{0} {1} {2}", Global.LoggedInPerson.Role.Name, Global.LoggedInPerson.Name, Global.LoggedInPerson.Surname);

                        //lblEventName.BackgroundColor = Color.Green;
                        @event.Colour = "Green";
                    }
                    else if (@event.ThirdRespondent == null)
                    {
                        @event.ThirdRespondent = Global.LoggedInPerson;
                        lblThirdRespondent.Text = String.Format("{0} {1} {2}", Global.LoggedInPerson.Role.Name, Global.LoggedInPerson.Name, Global.LoggedInPerson.Surname);
                    }
                    SetAlarm(@event);
                }
                else
                {
                    button.Text = "Confirm Attendance";
                    lblEventName.TextColor = Color.Red;
                    frmEventDetail.BorderColor = Color.Red;
                    btnAlarm.IsVisible = true;
                    btnAlarm.BackgroundColor = Color.Red;
                    button.BackgroundColor = Color.Red;
                    btnLocation.BackgroundColor = Color.Red;
                    @event.Colour = "Red";
                    @event.IsVisible = true;
                    @event.AlarmSet = false;
                    @event.AlarmText = "Set Alarm";

                    if (@event.FirstRespondent?.PersonID == Global.LoggedInPerson.PersonID)
                    {
                        @event.FirstRespondent = null;
                        lblFirstRespondent.Text = "No one";
                    }
                    else if (@event.SecondRespondent?.PersonID == Global.LoggedInPerson.PersonID)
                    {
                        @event.SecondRespondent = null;
                        lblSecondRespondent.Text = "No one";
                    }
                    else if (@event.ThirdRespondent?.PersonID == Global.LoggedInPerson.PersonID)
                    {
                        @event.ThirdRespondent = null;
                        lblThirdRespondent.Text = "No one";
                    }
                    @event.AlarmSet = false;
                    @event.AlarmText = "Set Alarm";
                }

                await viewModel.UpdateEvent(@event).ConfigureAwait(false);
                new FirebaseDBBase().GetFirebaseData();
            }
            catch (System.Exception) { }
            IsBusy = false;
        }

        private void button_Clicked_1(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Event @event = (Event)((EventDetailViewModel)btn.CommandParameter).Event;
            SetAlarm(@event);
        }

        private void SetAlarm(Event @event)
        {
            var ctx = Android.App.Application.Context;

            Intent intent = new Intent(Android.Provider.AlarmClock.ActionSetAlarm);
            intent.PutExtra(Android.Provider.AlarmClock.ExtraHour, @event.Time.Hour);
            intent.PutExtra(Android.Provider.AlarmClock.ExtraMinutes, @event.Time.Minute);
            intent.PutExtra(Android.Provider.AlarmClock.ExtraMessage, @event.Name);
            intent.PutExtra(Android.Provider.AlarmClock.ExtraSkipUi, true);
            intent.SetFlags(ActivityFlags.NewTask);

            ctx.StartActivity(intent);
        }

        private async void GetCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    await DisplayAlert($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}", "Location", "Cancel");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        private void btnLocation_Clicked(object sender, EventArgs e)
        {
            GetCurrentLocation();
        }
    }
}