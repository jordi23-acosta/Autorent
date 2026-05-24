using AUTORENT.ViewModels;

namespace AUTORENT.Pages
{
    public partial class OwnerEarningsPage : ContentPage
    {
        private readonly OwnerEarningsViewModel _viewModel;

        public OwnerEarningsPage()
        {
            InitializeComponent();
            _viewModel = new OwnerEarningsViewModel();
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
