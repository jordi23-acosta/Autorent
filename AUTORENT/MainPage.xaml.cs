using AUTORENT.ViewModels;

namespace AUTORENT
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel ViewModel => (MainViewModel)BindingContext;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Actualizar nombre del usuario por si cambió
            var authService = AUTORENT.Services.AuthService.Instance;
            if (authService.CurrentUser != null)
            {
                ViewModel.UserName = authService.CurrentUser.Name;
            }
            
            await ViewModel.LoadAvailableVehiclesAsync();
        }
    }
}
