using REICTEventScheduler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace REICTEventScheduler.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            if (Global.LoggedInPerson.Role.Name.ToString() == PersonRole.Imaam.ToString())
            {
                menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem {Id = MenuItemType.Events, Title="Events" },
                    new HomeMenuItem {Id = MenuItemType.Persons, Title="Users" },
                    new HomeMenuItem {Id = MenuItemType.Reports, Title="Reports" },
                    new HomeMenuItem {Id = MenuItemType.About, Title="About" }
                };
            }

            ListViewMenu.ItemsSource = menuItems;
            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await new MainPage().NavigateFromMenu(id);
            };
        }
    }
}