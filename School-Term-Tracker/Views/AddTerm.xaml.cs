using SchoolTermTracker.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolTermTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        public AddTerm()
        {
            InitializeComponent();
        }

        async void save_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(termName.Text))
            {
                if (startDatePicker.Date <= endDatePicker.Date)
                {
                    await DBservice.AddTerm(termName.Text, startDatePicker.Date, endDatePicker.Date);
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