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
    [DesignTimeVisible(false)]
    public partial class PersonsPage : ContentPage
    {
        //private readonly EventsViewModel viewModel;
        //public bool ShowButton = false;

        public Person SelectedPerson { get; set; }

        public PersonsPage()
        {
            InitializeComponent();

            //BindingContext = viewModel = new EventsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            lstPersons.ItemsSource = Global.GlobalREICTModel.Persons;
            lstRole.ItemsSource = GetAllRoles();
        }

        private List<String> GetAllRoles()
        {
            var roles = new List<String>();
            foreach (var person in Global.GlobalREICTModel.Persons)
            {
                roles.Add(person.Role.Name );
            }
            return roles.Distinct().ToList();
        }

        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            bool personExists = false;
            foreach (var person in Global.GlobalREICTModel.Persons)
            {
                if (person.CellNumber == txtCellNumber.Text)
                {
                    SelectedPerson = person;
                    person.Name = txtName.Text;
                    person.Surname = txtSurname.Text;
                    person.CellNumber = txtCellNumber.Text;
                    person.PersonID = Guid.NewGuid().ToString();
                    person.Password = txtPassword.Text;
                    person.Role.Name = lstRole.SelectedItem.ToString();
                    personExists = true;
                    break;
                }
            }

            if (!personExists)
            {
                var newPerson = new Person
                {
                    Name = txtName.Text,
                    Surname = txtSurname.Text,
                    CellNumber = txtCellNumber.Text,
                    PersonID = Guid.NewGuid().ToString(),
                    Password = txtPassword.Text
                };
                var role = new Role
                {
                    RoleID = Guid.NewGuid().ToString(),
                    Name = lstRole.SelectedItem.ToString()
                };
                newPerson.Role = role;

                Global.GlobalREICTModel.Persons.Add(newPerson);
            }

            await FirebaseDBBase.UpdatePersonAsync(Global.GlobalREICTModel);

            ClearForm();
        }
        
        private void lstPersons_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItemIndex != -1)
            {
                var person = (Person)e.SelectedItem;
                SelectedPerson = person;

                txtName.Text = person.Name;
                txtSurname.Text = person.Surname;
                txtCellNumber.Text = person.CellNumber;
                txtPassword.Text = person.Password;
                //txtRole.Text = person.Role.Name;
                lstPersons.SelectedItem = 0;
                btnAdd.Text = "Update";
                btnCancel.IsVisible = true;
            }
        }

        private void ClearForm()
        {
            btnAdd.Text = "Add Person";
            txtName.Text = "";
            txtSurname.Text = "";
            txtPassword.Text = "";
            //txtRole.Text = "";
            txtCellNumber.Text = "";
            btnCancel.IsVisible = false;
            btnAdd.IsEnabled = false;
            SelectedPerson = null;
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedPerson != null)
            {
                if ((!string.IsNullOrWhiteSpace(txtName.Text) && txtName.Text.Trim() != SelectedPerson.Name) ||
                    (!string.IsNullOrWhiteSpace(txtSurname.Text) && txtSurname.Text.Trim() != SelectedPerson.Surname) ||
                    (!string.IsNullOrWhiteSpace(txtCellNumber.Text) && txtCellNumber.Text.Trim() != SelectedPerson.CellNumber) ||
                    (!string.IsNullOrWhiteSpace(txtPassword.Text) && txtPassword.Text.Trim() != SelectedPerson.Password))// ||
                    //(!string.IsNullOrWhiteSpace(txtRole.Text) && txtRole.Text.Trim() != SelectedPerson.Role.Name))
                {
                    btnAdd.IsEnabled = true;
                    btnCancel.IsVisible = true;
                }
                else
                {
                    btnAdd.IsEnabled = false;
                    //btnCancel.IsVisible = false;
                }
            }
            else
            {
                if ((!string.IsNullOrWhiteSpace(txtName.Text) &&
                    !string.IsNullOrWhiteSpace(txtSurname.Text) &&
                    !string.IsNullOrWhiteSpace(txtCellNumber.Text) &&
                    !string.IsNullOrWhiteSpace(txtPassword.Text) 
                    ))
                {
                    btnAdd.IsEnabled = true;
                    btnCancel.IsVisible = true;
                }
                else
                {
                    btnAdd.IsEnabled = false;
                    //btnCancel.IsVisible = false;
                }
            }
        }

    }
}