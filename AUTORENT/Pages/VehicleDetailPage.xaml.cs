using AUTORENT.Models;
using AUTORENT.ViewModels;

namespace AUTORENT.Pages
{
    public partial class VehicleDetailPage : ContentPage
    {
        private readonly VehicleDetailViewModel _viewModel;

        public VehicleDetailPage(Vehicle vehicle)
        {
            InitializeComponent();
            _viewModel = new VehicleDetailViewModel(vehicle);
            BindingContext = _viewModel;
        }

        private async void OnBackTapped(object? sender, TappedEventArgs e)
        {
            await _viewModel.NavigateBackAsync();
        }
    }
}
