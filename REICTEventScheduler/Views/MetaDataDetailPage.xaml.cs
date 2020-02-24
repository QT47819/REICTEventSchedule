using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using REICTEventScheduler.Models;
using REICTEventScheduler.ViewModels;
using REICTEventScheduler.Services;

namespace REICTEventScheduler.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MetaDataDetailPage : ContentPage
    {
        EventDetailViewModel viewModel;
        static Countdown countdown;
        static TimeSpan timeSpan;

        public MetaDataDetailPage(EventDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            if (viewModel.Event.IsVisible == false)
                button.Text = "Reject Attendance";
            else
                button.Text = "Confirm Attendance";

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
            try
            {
                IsBusy = true;

                Button btn = (Button)sender;

                var vm = (EventDetailViewModel)btn.CommandParameter;
                Event @event = vm.Event;
                
                if (viewModel.Event.IsVisible == true)
                {
                    button.Text = "Reject Attendance";
                    @event.IsVisible = false;

                    if (@event.FirstRespondent == null)
                    {
                        @event.FirstRespondent = Global.LoggedInPerson;
                        lblFirstRespondent.Text = String.Format("{0} {1} {2}", Global.LoggedInPerson.Role.Name, Global.LoggedInPerson.Name, Global.LoggedInPerson.Surname);

                        lblEventName.BackgroundColor = Color.Orange;
                        @event.Colour = "Orange";
                    }
                    else if (@event.SecondRespondent == null)
                    {
                        @event.SecondRespondent = Global.LoggedInPerson;
                        lblSecondRespondent.Text = String.Format("{0} {1} {2}", Global.LoggedInPerson.Role.Name, Global.LoggedInPerson.Name, Global.LoggedInPerson.Surname);

                        lblEventName.BackgroundColor = Color.Green;
                        @event.Colour = "Green";
                    }
                    else if (@event.ThirdRespondent == null)
                    {
                        @event.ThirdRespondent = Global.LoggedInPerson;
                        lblThirdRespondent.Text = String.Format("{0} {1} {2}", Global.LoggedInPerson.Role.Name, Global.LoggedInPerson.Name, Global.LoggedInPerson.Surname);
                    }
                }
                else
                {
                    button.Text = "Confirm Attendance";
                    lblEventName.BackgroundColor = Color.Red;
                    @event.Colour = "Red";
                    @event.IsVisible = true;

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
                }

                await viewModel.UpdateEvent(@event).ConfigureAwait(false);
                new FirebaseDBBase().GetFirebaseData();
            }
            catch (System.Exception) { }
            IsBusy = false;
        }
    }
}