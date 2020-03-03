using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using REICTEventScheduler.Models;

namespace REICTEventScheduler.Services
{
    public class FirebaseDBBase //: INotifyPropertyChanged
    {
        public static List<REICTModel> REICTModel;
        readonly static FirebaseClient firebase = new FirebaseClient("https://reiteventscheduler.firebaseio.com/");
        //string TypeName;
        public static event PropertyChangedEventHandler PropertyChanged;
        private static bool _isBusy;

        private static string salaahTimesURL;

        public static string SalaahTimesURL
        {
            get { return salaahTimesURL; }
            set { salaahTimesURL = $"http://api.aladhan.com/v1/calendarByCity"; }
        }

        public static bool isBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                //NotifyPropertyChanged();
            }
        }

        public FirebaseDBBase()
        {
            GetFirebaseData();
        }

        public void GetFirebaseData()
        {
            GetItemsAsync(false).ConfigureAwait(false);
        }

        public static async Task<bool> AddItemAsync(REICTModel rEICTModel)
        {
            //REICTModel.Add(rEICTModel);
            await UpdateEventAsync(rEICTModel).ConfigureAwait(false); ;
            return await Task.FromResult(true);
        }

        private static bool PersonInfoChanged(Person personToCheck, FirebaseObject<Person> fbPerson)
        {
            if (personToCheck.Name != fbPerson.Object.Name)
                return true;
            if (personToCheck.Surname != fbPerson.Object.Surname)
                return true;
            if (personToCheck.CellNumber != fbPerson.Object.CellNumber)
                return true;
            if (personToCheck.Password != fbPerson.Object.Password)
                return true;
            if (personToCheck.Role.Name != fbPerson.Object.Role.Name)
                return true;

            return false;
        }

        public static async Task<bool> UpdatePersonAsync(REICTModel rEICTModel)
        {
            foreach (var person in rEICTModel.Persons)
            {
                var toUpdatePerson = (await firebase
                    .Child("Persons")
                    .OnceAsync<Person>()).Where(a => a.Object.PersonID == person.PersonID).FirstOrDefault();

                if (toUpdatePerson == null)
                {
                    await firebase
                          .Child("Persons")
                          .PostAsync(new Person()
                          {
                              PersonID = person.PersonID,
                              Name = person.Name,
                              Surname = person.Surname,
                              CellNumber = person.CellNumber,
                              Password = person.Password,
                              Role = person.Role
                          }).ConfigureAwait(false);
                }
                else if (PersonInfoChanged(person, toUpdatePerson))
                {
                    await firebase
                            .Child("Persons")
                            .Child(toUpdatePerson.Key)
                            .PutAsync(new Person()
                            {
                                PersonID = person.PersonID,
                                Name = person.Name,
                                Surname = person.Surname,
                                CellNumber = person.CellNumber,
                                Password = person.Password,
                                Role = person.Role
                            }).ConfigureAwait(false);
                }
            }

            return await Task.FromResult(true);
        }

        private static string RemoveDayFormatter(string Name, DateTime waqtuTime)
        {
            string todayDateFormatter = waqtuTime.ToString("");
            return Name.Replace(" Today", "").Replace(" Tomorrow", "").Replace(todayDateFormatter, "");
        }

        private static bool CheckIfEventhasChanged(Event evnt, FirebaseObject<Event> fbEvent)
        {
            if (RemoveDayFormatter(evnt.Name, evnt.Time) != RemoveDayFormatter(fbEvent.Object.Name, fbEvent.Object.Time))
                return true;
            if (evnt.Colour != fbEvent.Object.Colour)
                return true;
            if (evnt.Description != fbEvent.Object.Description)
                return true;
            if (evnt.FirstRespondent != fbEvent.Object.FirstRespondent)
                return true;
            if (evnt.IsVisible != fbEvent.Object.IsVisible)
                return true;
            if (evnt.SecondRespondent != fbEvent.Object.SecondRespondent)
                return true;
            if (evnt.ThirdRespondent != fbEvent.Object.ThirdRespondent)
                return true;
            if (evnt.Time != fbEvent.Object.Time)
                return true;

            return false;
        }

        public static async Task<bool> UpdateEventAsync(REICTModel rEICTModel)
        {
            foreach (var @event in rEICTModel.Events)
            {
                var toUpdateEvent = (await firebase
                    .Child("Events")
                    .OnceAsync<Event>()).Where(a => a.Object.Id == @event.Id).FirstOrDefault();

                if (toUpdateEvent != null)
                {
                    if(CheckIfEventhasChanged(@event, toUpdateEvent))
                        await firebase
                            .Child("Events")
                            .Child(toUpdateEvent.Key)
                            .PutAsync(new Event()
                            {
                                Id = @event.Id,
                                Description = @event.Description.Replace(" Today", "").Replace(" Tomorrow", "").Trim(),
                                Name = RemoveDayFormatter(@event.Name, @event.Time),
                                Time = @event.Time,
                                FirstRespondent = @event.FirstRespondent,
                                SecondRespondent = @event.SecondRespondent,
                                ThirdRespondent = @event.ThirdRespondent,
                                Colour = @event.Colour,
                                AlarmSet = @event.AlarmSet,
                                AlarmText = @event.AlarmText,
                                IsVisible = @event.IsVisible
                            }).ConfigureAwait(false);
                    }
                else
                {
                    await firebase
                          .Child("Events")
                          .PostAsync(new Event()
                          {
                              Id = @event.Id,
                              Description = @event.Description.Replace(" Today", "").Replace(" Tomorrow", "").Trim(),
                              Name = RemoveDayFormatter(@event.Name, @event.Time),
                              Time = @event.Time,
                              FirstRespondent = @event.FirstRespondent,
                              SecondRespondent = @event.SecondRespondent,
                              ThirdRespondent = @event.ThirdRespondent,
                              Colour = @event.Colour,
                              AlarmSet = @event.AlarmSet,
                              AlarmText = @event.AlarmText,
                              IsVisible = @event.IsVisible
                          }).ConfigureAwait(false);
                }
            }

            return await Task.FromResult(true);
        }

        public static async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = REICTModel.FirstOrDefault();
            REICTModel.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public static async Task<REICTModel> GetItemAsync(string id)
        {
            return await Task.FromResult(REICTModel.FirstOrDefault());
        }

        private static async Task<List<Person>> GetPersonsFromFirebase(string childName)
        {
            var child = (await firebase
                          .Child(childName)
                          .OnceAsync<Person>().ConfigureAwait(true)).Select(item => item.Object).ToList();
            return child;
        }

        private static async Task<List<Setting>> GetSettingsFromFirebase(string childName)
        {
            var child = (await firebase
                          .Child(childName)
                          .OnceAsync<Setting>().ConfigureAwait(true)).Select(item => item.Object).ToList();
            return child;
        }

        private static async Task<List<SalaahTime>> GetSalaahTimesFromFirebase(string childName)
        {
            var child = (await firebase
                          .Child(childName)
                          .OnceAsync<SalaahTime>().ConfigureAwait(true)).Select(item => item.Object).ToList();
            return child;
        }

        private static async Task<List<Event>> GetEventsFromFirebase(string childName)
        {
            var child = (await firebase
                          .Child(childName)
                          .OnceAsync<Event>().ConfigureAwait(true)).Select(item => item.Object).ToList();
            return child;
        }

        public static async Task AddSalaahTime()
        {
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();
            string search = "&month=" + month + "&year=" + year;

            var salaahTime = new SalaahTime
            {
                status = search,
                date = DateTime.Now, //Parse(String.Format("1 {0} {1}", month, year)),
                code = 0
            };

            await AddSalaahTimeforMonth(salaahTime).ConfigureAwait(false);
        }

        private static async Task AddSalaahTimeforMonth(SalaahTime salaahTime)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri($"http://api.aladhan.com/v1/calendarByCity")
            };

            var result = await client.GetStringAsync("?city=Cape Town&country=South Africa&method=2" + salaahTime.status).ConfigureAwait(true);
            SalaahTime salaahTimes = await Task.Run(() => JsonConvert.DeserializeObject<SalaahTime>(result));

            if (salaahTimes.status == "OK")
            {
                await firebase
                  .Child("SalaahTimes")
                  .PostAsync(new SalaahTime() { data = salaahTimes.data, date = salaahTime.date, code = salaahTime.code, status = salaahTime.status });
            }
        }

        public static async Task GetItemsAsync(bool forceRefresh = false)
        {
            isBusy = true;

            var personsTask = GetPersonsFromFirebase("Persons");
            var settingsTask = GetSettingsFromFirebase("Settings");
            var salaahTimesTask = GetSalaahTimesFromFirebase("SalaahTimes");
            var eventsTask = GetEventsFromFirebase("Events");

            var persons = new List<Person>();
            var events = new List<Event>();
            var settings = new List<Setting>();
            var salaahTimes = new List<SalaahTime>();

            var allTasks = new List<Task> { personsTask, settingsTask, salaahTimesTask, eventsTask };
            while (allTasks.Any())
            {
                Task finished = await Task.WhenAny(allTasks);
                if (finished == personsTask)
                {
                    persons = personsTask.Result;
                    allTasks.Remove(personsTask);
                }
                if (finished == salaahTimesTask)
                {
                    salaahTimes = salaahTimesTask.Result;
                    allTasks.Remove(salaahTimesTask);
                }
                if (finished == eventsTask)
                {
                    events = eventsTask.Result;
                    allTasks.Remove(eventsTask);
                }
                if (finished == settingsTask)
                {
                    settings = settingsTask.Result;
                    allTasks.Remove(settingsTask);
                }
            }

            Global.GlobalREICTModel.Persons = persons;
            Global.GlobalREICTModel.Events = events;
            Global.GlobalREICTModel.Settings = settings;

            bool monthFound = false;

            foreach (var st in salaahTimes)
            {
                if (st.date.Month == DateTime.Now.Month && st.date.Year == DateTime.Now.Year)
                {
                    monthFound = true;
                    break;
                }
            }

            if (!monthFound)
            {
                await AddSalaahTime().ConfigureAwait(false);
                await GetItemsAsync(false);
            }

            if(Global.GlobalREICTModel.SalaahTimes == null || Global.GlobalREICTModel.SalaahTimes.Count < salaahTimes.Count)
                Global.GlobalREICTModel.SalaahTimes = salaahTimes;
                        
            isBusy = false;

        }
    }
}