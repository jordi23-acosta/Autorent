using AUTORENT.ViewModels;

namespace AUTORENT.Pages
{
    public partial class OwnerRentalsPage : ContentPage
    {
        private readonly OwnerRentalsViewModel _viewModel;

        public OwnerRentalsPage()
        {
            InitializeComponent();
            _viewModel = new OwnerRentalsViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }

        private async void OnBackTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (Navigation?.NavigationStack?.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
            catch { }
        }
    }
}
