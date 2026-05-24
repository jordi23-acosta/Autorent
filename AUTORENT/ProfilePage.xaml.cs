using AUTORENT.ViewModels;

namespace AUTORENT
{
    public partial class ProfilePage : ContentPage
    {
        private ProfileViewModel ViewModel => (ProfileViewModel)BindingContext;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadUserData();
        }
    }
}
