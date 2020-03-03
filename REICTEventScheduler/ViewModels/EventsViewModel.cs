using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using REICTEventScheduler.Models;
using REICTEventScheduler.Views;
using REICTEventScheduler.Services;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace REICTEventScheduler.ViewModels
{
    public class EventsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private ObservableCollection<Event> events;
        public ObservableCollection<Event> Events
        {
            get
            {
                return events;
            }

            set
            {
                if (value != events)
                {
                    events = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Command LoadEventsCommand { get; set; }
        public int iEvents { get; set; }

        static Countdown countdown;
        static TimeSpan timeSpan;
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public EventsViewModel()
        {
            Title = string.Format("Top {0} Current Events", Global.NumberOfEvents + 1);
            Events = new ObservableCollection<Event>();
            LoadEventsCommand = new Command(async () => ExecuteLoadEventsCommand());
            iEvents = 0;

            MessagingCenter.Subscribe<NewEventPage, Event>(this, "AddEvent", async (obj, @event) =>
            {
                var newEvent = @event as Event;
                Events.Add(newEvent);
                Global.GlobalREICTModel.Events.Add(@event);
                await FirebaseDBBase.AddItemAsync(Global.GlobalREICTModel);
            });
        }

        public string AddFormatEventName(string eventName, DateTime eventTime, int eventNumber, bool addEventNumber = false)
        {
            string formattedEvent = RemoveFormatEventName(eventName, eventTime);
            string dayFormatter;

            if (eventTime.DayOfWeek == DayOfWeek.Friday)
            {
                if (formattedEvent.ToUpper() == "DHUHR")
                    formattedEvent = "Jumuah";
            }
            else
            {
                DateTime DhuhrTime = DateTime.Parse(eventTime.ToString("dd MMMM yyyy " + "13:00"));
                eventTime = DhuhrTime;
            }

            eventName = formattedEvent.Trim();
            if (eventTime.ToString("dd MMMM yyyy") == DateTime.Now.ToString("dd MMMM yyyy"))
                dayFormatter = eventName + " Today";
            else if (eventTime.ToString("dd MMMM yyyy") == DateTime.Now.AddDays(1).ToString("dd MMMM yyyy"))
                dayFormatter = eventName + " Tomorrow";
            else
                dayFormatter = eventName + " " + eventTime.ToString("dd MMMM yyyy");

            if (addEventNumber)
                return eventNumber + ": " + dayFormatter;
            else
                return dayFormatter;
        }

        private string RemoveFormatEventName(string eventName, DateTime eventTime)
        {
            string eventNameWithNoNumber;
            try
            {
                eventNameWithNoNumber =  eventName.Split(':')[1].Trim();
            }
            catch (System.Exception)
            {
                eventNameWithNoNumber = eventName;
            }

            string todayDateFormatter = eventTime.ToString("dd MMMM yyyy");
            return eventNameWithNoNumber.Replace(" Today", "").Replace(" Tomorrow", "").Replace(todayDateFormatter, "").Trim();
        }

        private void CleanEventName()
        {
            foreach (Event @Event in Global.GlobalREICTModel.Events)
            {
                @Event.Name = RemoveFormatEventName(@Event.Name, @Event.Time);
            }
        }

        public async Task UpdateEvent(Event @event)
        {
            IsBusy = true;
            bool eventFound = false;
            @event.Name = RemoveFormatEventName(@event.Name, @event.Time);
            if (Global.GlobalREICTModel.Events.Count() > 0)
            {
                foreach (Event @Event in Global.GlobalREICTModel.Events)
                {
                    if (@event.Id == @Event.Id)
                    {
                        @Event.Name = @event.Name;
                        @Event.Colour = @event.Colour;
                        @Event.IsVisible = @event.IsVisible;
                        @Event.FirstRespondent = @event.FirstRespondent;
                        @Event.SecondRespondent = @event.SecondRespondent;
                        @Event.ThirdRespondent = @event.ThirdRespondent;
                        @Event.AlarmSet = @event.AlarmSet;
                        @Event.AlarmText = @event.AlarmText;
                        eventFound = true;
                    }
                }
            }
            if(!eventFound)
                Global.GlobalREICTModel.Events.Add(@event);

            CleanEventName();

            await FirebaseDBBase.UpdateEventAsync(Global.GlobalREICTModel);
            IsBusy = false;
        }

        public static void SetCountdown(object value, Label lblCountDownTimer)
        {
            timeSpan = DateTime.Parse(value.ToString()) - DateTime.Now;

            countdown = new Countdown();
            countdown.StartUpdating(timeSpan.TotalSeconds);

            lblCountDownTimer.SetBinding(Label.TextProperty,
                new Binding("RemainTime", BindingMode.Default, new CountdownConverter()));
            
            lblCountDownTimer.BindingContext = countdown;
        }

        private bool CheckIfEventIsAlreadyAdded(DateTime waqtuTime, string waqtuName)
        {
            foreach (var eventTime in Global.GlobalREICTModel.Events.Where(x => x.Time >= DateTime.Now.Subtract(new TimeSpan(0, 0, 5, 0))))
            {
                if (RemoveFormatEventName(eventTime.Name, eventTime.Time) == RemoveFormatEventName(waqtuName, waqtuTime) && eventTime.Time == waqtuTime)
                {
                    return true;
                }
            }
            return false;
        }

        private bool AddWaqtu(DateTime waqtuTime, string waqtuName, ref int eventNumber)
        {
            bool EventsFull = false;
            try
            {
                if (!CheckIfEventIsAlreadyAdded(waqtuTime, AddFormatEventName(waqtuName, waqtuTime, eventNumber)))
                {
                    if (DateTime.Now <= waqtuTime)
                    {
                        Events.Add(new Event()
                        {
                            Description = "Salaah: " + waqtuName,
                            Id = Guid.NewGuid().ToString(),
                            Name = AddFormatEventName(waqtuName, waqtuTime, eventNumber++),
                            Time = waqtuTime,
                            IsVisible = true,
                            AlarmSet = false,
                            AlarmText = "Set Alarm",
                            Colour = "Red"
                        });
                        if (iEvents < Global.NumberOfEvents)
                        {
                            iEvents++;
                        }
                        else
                        {
                            EventsFull = true;
                        }
                    }
                }
            }
            catch (SystemException ex) { }
            return EventsFull;
        }

        void ExecuteLoadEventsCommand(bool ForceRefresh = false)
        {
            //if (!ForceRefresh && Global.GlobalREICTModel.Events != null)
            //    return;

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                int iEventNumber = 1;

                Events.Clear();
                var events = Global.GlobalREICTModel.Events;
                var eventToAdd = new Event();
                bool EventsFull = false;

                foreach (var eventTime in Global.GlobalREICTModel.Events.Where(x => x.Time >= DateTime.Now.Subtract(new TimeSpan(0, 0, 5, 0))))
                {
                    if (eventTime.FirstRespondent?.PersonID == Global.LoggedInPerson.PersonID)
                    {
                        eventTime.IsVisible = false;
                        eventTime.Colour = "Orange";
                    }
                    if (eventTime.SecondRespondent?.PersonID == Global.LoggedInPerson.PersonID)
                    {
                        eventTime.IsVisible = false;
                        eventTime.Colour = "Orange";
                    }
                    if (eventTime.ThirdRespondent?.PersonID == Global.LoggedInPerson.PersonID)
                    {
                        eventTime.IsVisible = false;
                        eventTime.Colour = "Orange";
                    }

                    if(eventTime.SecondRespondent != null)
                        eventTime.Colour = "Green";

                    eventTime.Name = AddFormatEventName(eventTime.Name, eventTime.Time, iEventNumber++);

                    if (iEvents < Global.NumberOfEvents)
                    {
                        iEvents++;
                    }
                    else
                    {
                        EventsFull = true;
                    }

                    Events.Add(eventTime);
                }

                foreach (var salaah in Global.GlobalREICTModel.SalaahTimes)
                {
                    foreach (var salaahTime in salaah.data.Where(x => DateTime.Parse(x.date.readable) >= DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0))))
                    {
                        if (!EventsFull)
                            EventsFull = AddWaqtu(DateTime.Parse(salaahTime.date.readable + " " + salaahTime.timings.Fajr.Replace(" (SAST)", "")), "Fajr", ref iEventNumber);
                        else
                            break;

                        if (!EventsFull)
                        {
                            DateTime eventTime = DateTime.Parse(salaahTime.date.readable + " " + salaahTime.timings.Dhuhr.Replace(" (SAST)", ""));
                            string formattedEvent = "";
                            if (eventTime.DayOfWeek == DayOfWeek.Friday)
                            {
                                formattedEvent = "Jumuah";
                            }
                            else
                            {
                                DateTime DhuhrTime = DateTime.Parse(eventTime.ToString("dd MMMM yyyy " + "13:00"));
                                eventTime = DhuhrTime;
                                formattedEvent = "Dhuhr";
                            }

                            EventsFull = AddWaqtu(eventTime, formattedEvent, ref iEventNumber);
                        }
                        else
                            break;

                        if (!EventsFull)
                            EventsFull = AddWaqtu(DateTime.Parse(salaahTime.date.readable + " " + salaahTime.timings.Asr.Replace(" (SAST)", "")), "Asr", ref iEventNumber);
                        else
                            break;

                        if (!EventsFull)
                            EventsFull = AddWaqtu(DateTime.Parse(salaahTime.date.readable + " " + salaahTime.timings.Maghrib.Replace(" (SAST)", "")), "Maghrib", ref iEventNumber);
                        else
                            break;

                        if (!EventsFull)
                            EventsFull = AddWaqtu(DateTime.Parse(salaahTime.date.readable + " " + salaahTime.timings.Isha.Replace(" (SAST)", "")), "Isha", ref iEventNumber);
                        else
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                iEvents = 0;
                var eventOrder = Events.OrderBy(x => x.Time).ToList();
                Events.Clear();
                int evntNumber = 1;
                foreach (var evnt in eventOrder)
                {
                    evnt.Name = AddFormatEventName(evnt.Name, evnt.Time, evntNumber++, true);
                    Events.Add((Event)evnt);
                }
                IsBusy = false;
            }
        }
    }
}