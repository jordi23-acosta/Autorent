using AUTORENT.Models;
using AUTORENT.ViewModels;

namespace AUTORENT.Pages
{
    public partial class EditVehiclePage : ContentPage
    {
        private readonly EditVehicleViewModel _viewModel;

        public EditVehiclePage(Vehicle vehicle)
        {
            InitializeComponent();
            _viewModel = new EditVehicleViewModel(vehicle);
            BindingContext = _viewModel;
        }
    }
}
