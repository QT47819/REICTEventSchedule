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

namespace REICTEventScheduler.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ReportPage : ContentPage
    {
        //private readonly EventsViewModel viewModel;
        //public bool ShowButton = false;

        public ReportPage()
        {
            InitializeComponent();

            //BindingContext = viewModel = new EventsViewModel();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //lstReport.ItemsSource = Global.GlobalREICTModel.Persons;
        }

    }
}