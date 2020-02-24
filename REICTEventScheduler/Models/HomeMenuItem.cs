using System;
using System.Collections.Generic;
using System.Text;

namespace REICTEventScheduler.Models
{
    public enum MenuItemType
    {
        Persons,
        Events,
        Reports,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }
        public string Title { get; set; }
    }
}
