using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using REICTEventScheduler.Models;
using REICTEventScheduler.Views;
using REICTEventScheduler.ViewModels;
using REICTEventScheduler.Services;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace REICTEventScheduler.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class EventPage : ContentPage
    {
        private readonly EventsViewModel viewModel;
        public bool Updated { get; set; }

        public EventPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new EventsViewModel();
        }

        async void OnEventSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Event @event))
                return;

            await Navigation.PushAsync(new EventDetailPage(new EventDetailViewModel(@event)));

            // Manually deselect Event.
            EventsListView.SelectedItem = null;
        }

        async void AddEvent_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewEventPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadEventsCommand.Execute(null);
            Updated = false;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;

            bool setAlarm = false;

            Xamarin.Forms.Button btn = (Xamarin.Forms.Button)sender;
            Event @event = (Event)btn.CommandParameter;

            Color color = Color.Green;

            //btn.IsVisible = false;
            //btn.TextColor = color;

            try
            {
                if (@event.FirstRespondent == null)
                {
                    @event.FirstRespondent = Global.LoggedInPerson;
                    color = Color.Orange;
                    @event.Colour = "Orange";
                }
                else if (@event.SecondRespondent == null)
                {
                    @event.SecondRespondent = Global.LoggedInPerson;
                    color = color = Color.Green;
                    @event.Colour = "Green";
                }
                else if (@event.ThirdRespondent == null)
                {
                    @event.ThirdRespondent = Global.LoggedInPerson;
                    color = color = Color.Green;
                    @event.Colour = "Green";
                }

                @event.IsVisible = false;

                //var grd = (Grid)btn.Parent;
                //foreach (var c in grd.Children)
                //{
                //    if (c.GetType() == typeof(Label))
                //    {
                //        Label lbl = (Label)c;
                //        if (lbl.Text == @event.Name)
                //        {
                //            lbl.TextColor = color;
                //        }
                //        else if (lbl.Text == "Set Alarm")
                //        {
                //            lbl.Text = "Alarm has been set";
                //        }
                //    }
                //    else if (c.GetType() == typeof(Xamarin.Forms.CheckBox))
                //    {
                //        Xamarin.Forms.CheckBox checkBox = (Xamarin.Forms.CheckBox)c;
                //        if (checkBox.IsChecked)
                //        {
                //            checkBox.IsEnabled = false;
                //            checkBox.IsVisible = false;
                //            setAlarm = true;
                //        }
                //    }
                //    else if (c.GetType() == typeof(Xamarin.Forms.Button))
                //    {
                //        if (!setAlarm)
                //        {
                //            Xamarin.Forms.Button AlarmButton = (Xamarin.Forms.Button)c;
                //            if (AlarmButton.Text == @event.AlarmText)
                //            {
                //                AlarmButton.IsVisible = true;
                //                AlarmButton.TextColor = color;
                //                AlarmButton.CornerRadius = 15;

                //                grd.Children.Remove(AlarmButton);
                //                grd.Children.Remove(btn);
                //                grd.Children.Add(AlarmButton, 0, 4);
                //                Grid.SetColumnSpan(AlarmButton, 3);
                //                break;
                //            }
                //        }
                //    }

                //    //c.BackgroundColor = color;
                //    //break;
                //}

                //var frame = (Frame)grd.Parent;
                //frame.BorderColor = color;

                //if (setAlarm)
                //{
                    @event.AlarmSet = true;
                    @event.AlarmText = "Alarm has been set";
                    //SetAlarm(@event);
                //}

                int iEventNumber = int.Parse(@event.Name.Split(':')[0]);
                await viewModel.UpdateEvent(@event).ConfigureAwait(false);
                //EventsListView.ItemsSource = null;
                //viewModel.LoadEventsCommand.Execute(null);
                @event.Name = viewModel.AddFormatEventName(@event.Name, @event.Time, iEventNumber);
                SetAlarm((Event)btn.CommandParameter);
            }
            catch (System.Exception ex) 
            { }

            IsBusy = false;            
            Updated = true;
        }

        //private void Button_Clicked_1(object sender, EventArgs e)
        //{
        //    Xamarin.Forms.Button btn = (Xamarin.Forms.Button)sender;

        //    //SetAlert();
        //    SetAlarm((Event)btn.CommandParameter);
        //    //DeleteAlarm();
        //}

        private void SetAlert()
        {
            //GET TIME IN SECONDS AND INITIALIZE INTENT
            int time = 20; //Convert.ToInt32(timeTxt.Text);
            Intent i = new Intent(Android.App.Application.Context, typeof(AlarmReceiver));

            //PASS CONTEXT,YOUR PRIVATE REQUEST CODE,INTENT OBJECT AND FLAG
            PendingIntent pi = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, i, 0);

            //INITIALIZE ALARM MANAGER
            var ctx = Android.App.Application.Context;
            AlarmManager alarmManager = (AlarmManager)ctx.GetSystemService(Context.AlarmService);

            //SET THE ALARM
            alarmManager.Set(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + 5 * 1000, pi);
            Toast.MakeText(Android.App.Application.Context, "Alarm set In: " + time + " seconds", ToastLength.Short).Show();
        }

        private void DeleteAlarm()
        {
            var ctx = Android.App.Application.Context;
            Intent intent = new Intent(Android.Provider.AlarmClock.ActionDismissAlarm);
            intent.SetFlags(ActivityFlags.NewTask);
            ctx.StartActivity(intent);
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
    }
}