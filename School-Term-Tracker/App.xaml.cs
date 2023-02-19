using Xamarin.Forms;

namespace SchoolTermTracker
{
    public partial class App : Application
    {
        public static string FilePath;
        public App()
        {
            InitializeComponent();
            
            MainPage = new NavigationPage(new Terms());          
        }

        protected override void OnStart()
        {
           
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
