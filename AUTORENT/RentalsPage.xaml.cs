using AUTORENT.ViewModels;

namespace AUTORENT
{
    public partial class RentalsPage : ContentPage
    {
        private RentalsViewModel ViewModel => (RentalsViewModel)BindingContext;

        public RentalsPage()
        {
            InitializeComponent();
            BindingContext = new RentalsViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.LoadRentalsAsync();
        }
    }
}
