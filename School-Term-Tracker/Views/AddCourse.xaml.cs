using SchoolTermTracker.Models;
using SchoolTermTracker.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolTermTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {
        public AddCourse(Term selectedTerm)
        {
            InitializeComponent();
            DBservice.CurrentTerm = selectedTerm;
            termIDLink.Text = selectedTerm.TermID.ToString();
            courseStatusPicker.SelectedItem = "In Progress";
        }

        async void SaveCourse_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(courseName.Text) && !String.IsNullOrWhiteSpace(courseStatusPicker.ToString()) && !String.IsNullOrWhiteSpace(instructorName.Text)
            && !String.IsNullOrWhiteSpace(instructorPhone.Text) && !String.IsNullOrWhiteSpace(instructorEmail.Text) && !String.IsNullOrWhiteSpace(courseNotes.Text) && instructorEmail.Text.Contains("@"))
            {
                if (courseStartDatePicker.Date <= courseEndDatePicker.Date)
                {
                    await DBservice.AddCourse(courseName.Text, courseStartDatePicker.Date, courseEndDatePicker.Date, courseStatusPicker.SelectedItem.ToString(), instructorName.Text,
                        instructorPhone.Text, instructorEmail.Text, courseNotes.Text, Int32.Parse(termIDLink.Text), courseSwitch.IsToggled);
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Alert", "Start date must be on or before the end date", "OK");
                }
            }
            else
            {
                await DisplayAlert("Alert", "Please ensure valid data is entered", "OK");
            }

        }
    }
}