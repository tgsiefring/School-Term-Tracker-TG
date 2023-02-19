using SchoolTermTracker.Models;
using SchoolTermTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolTermTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Assessments : ContentPage
    {
        public Assessments(Course selectedCourse)
        {
            InitializeComponent();
            DBservice.CurrentCourse = selectedCourse;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            collectionViewAssessments.ItemsSource = await DBservice.GetAssessments(DBservice.CurrentCourse);           
        }      

        async void addAssessment_Clicked(object sender, EventArgs e)
        {
            IEnumerable<Assessment> assessments = await DBservice.GetAssessments(DBservice.CurrentCourse);

            if (assessments.ToList().Count < 2)
            {
                await Navigation.PushAsync(new AddAssessment(DBservice.CurrentCourse));
            }
            else
            {
                await DisplayAlert("Alert", "Unable to add more assessments", "OK");
            }
        }

        async void collectionViewAssessments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Assessment assessment = (Assessment)e.CurrentSelection.FirstOrDefault();
                await Navigation.PushAsync(new AssessmentInformation(assessment));
            }
        }
    }
}