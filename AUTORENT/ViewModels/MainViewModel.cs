using AUTORENT.Models;
using AUTORENT.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private ObservableCollection<Vehicle> _availableVehicles;

        public MainViewModel()
        {
            _authService = AuthService.Instance;
            _availableVehicles = new ObservableCollection<Vehicle>();
            
            Title = "Inicio";
            
            SearchCommand = new AsyncRelayCommand(SearchAsync);
            RefreshCommand = new AsyncRelayCommand(LoadAvailableVehiclesAsync);
            
            // Cargar vehículos al inicializar
            _ = LoadAvailableVehiclesAsync();
        }

        public ObservableCollection<Vehicle> AvailableVehicles
        {
            get => _availableVehicles;
            set => SetProperty(ref _availableVehicles, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        public async Task LoadAvailableVehiclesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null) return;

                // Cargar vehículos disponibles (máximo 10 para la página de inicio)
                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Where(x => x.IsAvailable == true)
                    .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(10)
                    .Get();

                AvailableVehicles.Clear();
                if (vehiclesResponse?.Models != null)
                {
                    foreach (var vehicle in vehiclesResponse.Models)
                    {
                        AvailableVehicles.Add(vehicle);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando vehículos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchAsync()
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Búsqueda", 
                "Función de búsqueda en desarrollo", 
                "OK");
        }

        public string GetVehicleEmoji(string brand)
        {
            return brand.ToLower() switch
            {
                var b when b.Contains("toyota") => "🚙",
                var b when b.Contains("honda") => "🚐",
                var b when b.Contains("bmw") => "🏎️",
                var b when b.Contains("mercedes") => "🚗",
                var b when b.Contains("ford") => "🚙",
                var b when b.Contains("chevrolet") => "🚙",
                var b when b.Contains("nissan") => "🚗",
                var b when b.Contains("mazda") => "🚗",
                _ => "🚗"
            };
        }
    }
}
