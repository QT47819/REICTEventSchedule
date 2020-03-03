using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using REICTEventScheduler.Models;

namespace REICTEventScheduler.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            //MenuPages.Add((int)MenuItemType.Events, (NavigationPage)Detail);
        }

        public void NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Events:
                        MenuPages.Add(id, new NavigationPage(new EventPage()));
                        break;
                    case (int)MenuItemType.Persons:
                        MenuPages.Add(id, new NavigationPage(new PersonsPage()));
                        break;
                    case (int)MenuItemType.Reports:
                        MenuPages.Add(id, new NavigationPage(new ReportPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null) // && Detail != newPage)
            {
                MainPage mainPage = (MainPage)Application.Current.MainPage;
                mainPage.Detail = newPage;

                IsPresented = true;
            }
        }
    }
}