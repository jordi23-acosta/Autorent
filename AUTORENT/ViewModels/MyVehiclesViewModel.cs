using AUTORENT.Services;
using AUTORENT.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class MyVehiclesViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        private ObservableCollection<Vehicle> _vehicles;
        private string _activeCount = "0";
        private string _requestsCount = "0";
        private string _earnings = "$0";
        private bool _hasVehicles;

        public MyVehiclesViewModel()
        {
            _authService = AuthService.Instance;
            _vehicles = new ObservableCollection<Vehicle>();
            
            Title = "Mis Vehículos";
            
            RefreshCommand = new AsyncRelayCommand(LoadVehiclesAsync);
            AddVehicleCommand = new AsyncRelayCommand(NavigateToAddVehicleAsync);
            ViewEarningsCommand = new AsyncRelayCommand(NavigateToEarningsAsync);
            ViewRentalsCommand = new AsyncRelayCommand(NavigateToRentalsAsync);
        }

        public ObservableCollection<Vehicle> Vehicles
        {
            get => _vehicles;
            set => SetProperty(ref _vehicles, value);
        }

        public string ActiveCount
        {
            get => _activeCount;
            set => SetProperty(ref _activeCount, value);
        }

        public string RequestsCount
        {
            get => _requestsCount;
            set => SetProperty(ref _requestsCount, value);
        }

        public string Earnings
        {
            get => _earnings;
            set => SetProperty(ref _earnings, value);
        }

        public bool HasVehicles
        {
            get => _hasVehicles;
            set => SetProperty(ref _hasVehicles, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddVehicleCommand { get; }
        public ICommand ViewEarningsCommand { get; }
        public ICommand ViewRentalsCommand { get; }

        public async Task LoadVehiclesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null || _authService.CurrentUser == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error", 
                        "No has iniciado sesión", 
                        "OK");
                    return;
                }

                var userId = _authService.CurrentUser.Id;

                // Cargar vehículos del propietario
                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Where(x => x.OwnerId == userId)
                    .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                Vehicles.Clear();
                if (vehiclesResponse?.Models != null)
                {
                    foreach (var vehicle in vehiclesResponse.Models)
                    {
                        Vehicles.Add(vehicle);
                    }
                }

                HasVehicles = Vehicles.Count > 0;

                // Actualizar estadísticas
                await UpdateStatisticsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando vehículos: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error", 
                    "No se pudieron cargar los vehículos", 
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdateStatisticsAsync()
        {
            try
            {
                var client = _authService.GetClient();
                if (client == null || _authService.CurrentUser == null) return;

                var userId = _authService.CurrentUser.Id;

                // Contar rentas activas
                var activeRentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.OwnerId == userId)
                    .Filter("status", Supabase.Postgrest.Constants.Operator.In, new[] { "activa", "confirmada" })
                    .Get();

                var activeCount = activeRentalsResponse?.Models?.Count ?? 0;
                ActiveCount = activeCount.ToString();

                // Contar solicitudes pendientes
                var pendingRentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.OwnerId == userId)
                    .Where(x => x.StatusString == "pendiente")
                    .Get();

                var pendingCount = pendingRentalsResponse?.Models?.Count ?? 0;
                RequestsCount = pendingCount.ToString();

                // Calcular ganancias totales
                var completedRentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.OwnerId == userId)
                    .Where(x => x.StatusString == "completada")
                    .Get();

                var totalEarnings = completedRentalsResponse?.Models?.Sum(r => r.TotalPrice) ?? 0;
                Earnings = totalEarnings >= 1000
                    ? $"${totalEarnings / 1000:F1}K"
                    : $"${totalEarnings:F0}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando estadísticas: {ex.Message}");
            }
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

        public Microsoft.Maui.Graphics.Color GetVehicleBackgroundColor(string brand)
        {
            return brand.ToLower() switch
            {
                var b when b.Contains("toyota") => Microsoft.Maui.Graphics.Color.FromArgb("#E3F2FD"),
                var b when b.Contains("honda") => Microsoft.Maui.Graphics.Color.FromArgb("#FFF3E0"),
                var b when b.Contains("bmw") => Microsoft.Maui.Graphics.Color.FromArgb("#F3E5F5"),
                var b when b.Contains("mercedes") => Microsoft.Maui.Graphics.Color.FromArgb("#E8F5E9"),
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#F5F5F5")
            };
        }

        private async Task NavigateToAddVehicleAsync()
        {
            await Shell.Current.GoToAsync("//AddVehiclePage");
        }

        private async Task NavigateToEarningsAsync()
        {
            await Shell.Current.GoToAsync("//OwnerEarningsPage");
        }

        private async Task NavigateToRentalsAsync()
        {
            await Shell.Current.GoToAsync("//OwnerRentalsPage");
        }
    }
}
