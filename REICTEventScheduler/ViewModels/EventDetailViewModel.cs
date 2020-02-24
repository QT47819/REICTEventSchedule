using System;
using System.Linq;
using System.Threading.Tasks;
using REICTEventScheduler.Models;
using REICTEventScheduler.Services;

namespace REICTEventScheduler.ViewModels
{
    public class EventDetailViewModel : BaseViewModel
    {
        public Event Event { get; set; }
        public string FirstRespondent { get; set; }
        public string SecondRespondent { get; set; }
        public string ThirdRespondent { get; set; }
        public EventDetailViewModel(Event @event = null)
        {
            Title = @event?.Name;
            Event = @event;
            SetRespondents(@event);
        }

        private void SetRespondents(Event @event)
        {
            FirstRespondent = @event.FirstRespondent == null ? "No one" : String.Format("{0} {1} {2}", @event.FirstRespondent.Role.Name, @event.FirstRespondent.Name, @event.FirstRespondent.Surname);
            SecondRespondent = @event.SecondRespondent == null ? "No one" : String.Format("{0} {1} {2}", @event.SecondRespondent.Role.Name, @event.SecondRespondent.Name, @event.SecondRespondent.Surname);
            ThirdRespondent = @event.ThirdRespondent == null ? "No one" : String.Format("{0} {1} {2}", @event.ThirdRespondent.Role.Name, @event.ThirdRespondent.Name, @event.ThirdRespondent.Surname);
        }

        public async Task UpdateEvent(Event @event)
        {
            IsBusy = true;
            bool eventFound = false;
            if (Global.GlobalREICTModel.Events.Count() > 0)
            {
                foreach (Event @Event in Global.GlobalREICTModel.Events)
                {
                    if (@event.Id == @Event.Id)
                    {
                        @Event.Colour = @event.Colour;
                        @Event.IsVisible = @event.IsVisible;
                        @Event.FirstRespondent = @event.FirstRespondent;
                        @Event.SecondRespondent = @event.SecondRespondent;
                        @Event.ThirdRespondent = @event.ThirdRespondent;
                        eventFound = true;
                    }
                }
            }
            if (!eventFound)
                Global.GlobalREICTModel.Events.Add(@event);

            SetRespondents(@event);

            await FirebaseDBBase.UpdateEventAsync(Global.GlobalREICTModel);
            IsBusy = false;
        }
    }
}
