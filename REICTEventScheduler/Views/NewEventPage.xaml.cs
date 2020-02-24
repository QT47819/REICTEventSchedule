using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using REICTEventScheduler.Models;
using System.Collections.ObjectModel;
using REICTEventScheduler.ViewModels;

namespace REICTEventScheduler.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class NewEventPage : ContentPage
    {
        //public ObservableCollection<Event> Event { get; set; }
        EventDetailViewModel viewModel;

        public NewEventPage()
        {
            InitializeComponent();

            BindingContext = viewModel;
            lstEvents.ItemsSource = Global.GlobalREICTModel.Events;

            //BindingContext = 

            //Event = new ObservableCollection<Event>();
            //foreach (var @event in Global.GlobalREICTModel.Events)
            //{
            //    Event.Add(@event);
            //}

            //lstEvents.ItemsSource = Event;
            //BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {

            //DateTime eventTime = DateTime.Parse(dpEventDate.Date.ToString("dd MMMM yyyy") + " " + tpEventTime.Time.ToString());
            //Person newPerson = new Person();
            //var Event = new Event() { Name = txtName.Text, ID = Guid.NewGuid(), FirstRespondent = newPerson, SecondRespondent = newPerson, ThirdRespondent = newPerson, Time = eventTime };
            //await firebaseHelper.Add(Event);
            //lblId.Text = string.Empty;
            //txtName.Text = string.Empty;
            //await DisplayAlert("Success", "Event Added Successfully", "OK");
            //var allEvents = await firebaseHelper.GetAll();
            //lstEvents.ItemsSource = allEvents;

            //new Event()
            //{
            //    Colour = "Red",
            //    Description = txtdescription.Text,
            //    Id = Guid.NewGuid().ToString(),
            //    IsVisible = true,
            //    Name = txtName.Text,
            //    Time = DateTime.Parse( dpEventDate.ToString() + " " + tpEventTime.ToString())
            //};

            MessagingCenter.Send(this, "AddEvent", new Event()
            {
                Colour = "Red",
                Description = txtdescription.Text,
                Id = Guid.NewGuid().ToString(),
                IsVisible = true,
                Name = txtName.Text,
                Time = DateTime.Parse(dpEventDate.Date.ToString("dd MMMM yyyy") + " " + tpEventTime.Time.ToString())
            });
            
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}