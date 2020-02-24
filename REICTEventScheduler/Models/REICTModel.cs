using System;
using System.Collections.Generic;
using System.Text;

namespace REICTEventScheduler.Models
{
    public class REICTModel
    {
        public List<Event> Events { get; set; }
        public List<Person> Persons { get; set; }
        public List<SalaahTime> SalaahTimes { get; set; }
        public List<Setting> Settings { get; set; }
    }

    public class Event
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public Person FirstRespondent { get; set; }
        public Person SecondRespondent { get; set; }
        public Person ThirdRespondent { get; set; }
        public bool IsVisible { get; set; }
        public string Colour { get; set; }
        public bool AlarmSet { get; set; }
        public string AlarmText { get; set; }
    }

    public class Person
    {
        public string CellNumber { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string PersonID { get; set; }
        public Role Role { get; set; }
        public string Surname { get; set; }
    }

    public class Role
    {
        public string RoleID { get; set; }
        public string Name { get; set; }
    }

    public class SalaahTime
    {
        public int code { get; set; }
        public Datum[] data { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
    }

    public class Datum
    {
        public Date date { get; set; }
        public Meta meta { get; set; }
        public Timings timings { get; set; }
    }

    public class Date
    {
        public Gregorian gregorian { get; set; }
        public Hijri hijri { get; set; }
        public string readable { get; set; }
        public string timestamp { get; set; }
    }

    public class Gregorian
    {
        public string date { get; set; }
        public string day { get; set; }
        public Designation designation { get; set; }
        public string format { get; set; }
        public Month month { get; set; }
        public Weekday weekday { get; set; }
        public string year { get; set; }
    }

    public class Designation
    {
        public string abbreviated { get; set; }
        public string expanded { get; set; }
    }

    public class Month
    {
        public string en { get; set; }
        public int number { get; set; }
    }

    public class Weekday
    {
        public string en { get; set; }
    }

    public class Hijri
    {
        public string date { get; set; }
        public string day { get; set; }
        public Designation designation { get; set; }
        public string format { get; set; }
        public Month month { get; set; }
        public Weekday weekday { get; set; }
        public string year { get; set; }
    }

    public class Meta
    {
        public float latitude { get; set; }
        public string latitudeAdjustmentMethod { get; set; }
        public float longitude { get; set; }
        public Method method { get; set; }
        public string midnightMode { get; set; }
        public Offset offset { get; set; }
        public string school { get; set; }
        public string timezone { get; set; }
    }

    public class Method
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Offset
    {
        public int Asr { get; set; }
        public int Dhuhr { get; set; }
        public int Fajr { get; set; }
        public int Imsak { get; set; }
        public int Isha { get; set; }
        public int Maghrib { get; set; }
        public int Midnight { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }

    public class Timings
    {
        public string Asr { get; set; }
        public string Dhuhr { get; set; }
        public string Fajr { get; set; }
        public string Imsak { get; set; }
        public string Isha { get; set; }
        public string Maghrib { get; set; }
        public string Midnight { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
    }

    public class Setting
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}