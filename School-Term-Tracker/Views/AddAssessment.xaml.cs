using SchoolTermTracker.Models;
using SchoolTermTracker.Services;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolTermTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessment : ContentPage
    {
        public AddAssessment(Course selectedCourse)
        {
            InitializeComponent();
            DBservice.CurrentCourse = selectedCourse;
            courseIDLink.Text = selectedCourse.CourseID.ToString();
            assessmentTypePicker.SelectedItem = "Objective Assessment";
        }

        async void saveAssessment_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(assessmentName.Text) && !String.IsNullOrWhiteSpace(assessmentTypePicker.ToString()))
            {
                if (assessmentStartDatePicker.Date <= assessmentEndDatePicker.Date)
                {
                    var assessments = await DBservice.GetAssessments(DBservice.CurrentCourse);
                    
                    string type = assessmentTypePicker.SelectedItem.ToString();
                    if (assessments.Any(assessment => assessment.AssessmentType == type))
                    {
                        await DisplayAlert("Alert", "Unable to add multiple assessments of the same type", "OK");
                        return;
                    }
                    else
                    {
                        await DBservice.AddAssessment(assessmentName.Text, assessmentStartDatePicker.Date, assessmentEndDatePicker.Date, assessmentTypePicker.SelectedItem.ToString(),
                            Int32.Parse(courseIDLink.Text), assessmentSwitch2.IsToggled);
                        await Navigation.PopAsync();
                    }
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