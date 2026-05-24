using AUTORENT.ViewModels;

namespace AUTORENT.Pages
{
    public partial class MyVehiclesPage : ContentPage
    {
        private readonly MyVehiclesViewModel _viewModel;

        public MyVehiclesPage()
        {
            InitializeComponent();
            _viewModel = new MyVehiclesViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadVehiclesAsync();
        }
    }
}
