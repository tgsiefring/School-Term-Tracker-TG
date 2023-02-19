using SchoolTermTracker.Models;
using SchoolTermTracker.Services;
using SchoolTermTracker.Views;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolTermTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Terms : ContentPage
    {   
        //Used to ensure the NotificationCheck function only runs once
        public bool notifications = true;
        //Used to ensure the function DummyData only runs once
        int runOnce = 0;
        public Terms()
        {
            InitializeComponent();          
        }

        async void AddTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTerm());
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();                 
            await DBservice.DummyData(runOnce);
            collectionViewTerms.ItemsSource = await DBservice.GetTerms();
            _ = DBservice.NotificationCheck(notifications);
            runOnce = 1;
            notifications = false;
        }

        async void collectionViewTerms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Term term = (Term)e.CurrentSelection.FirstOrDefault();
                await Navigation.PushAsync(new TermInformation(term));              
            }           
        }  
    }
}