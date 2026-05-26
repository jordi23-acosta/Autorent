using AUTORENT.Services;
using AUTORENT.Models;
using AUTORENT.Pages;
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
        private string _totalCount = "0";
        private bool _hasVehicles;
        private string _userName = string.Empty;

        public MyVehiclesViewModel()
        {
            _authService = AuthService.Instance;
            _vehicles = new ObservableCollection<Vehicle>();
            
            Title = "Mis Vehículos";
            
            RefreshCommand = new AsyncRelayCommand(LoadVehiclesAsync);
            AddVehicleCommand = new AsyncRelayCommand(NavigateToAddVehicleAsync);
            ViewEarningsCommand = new AsyncRelayCommand(NavigateToEarningsAsync);
            ViewRentalsCommand = new AsyncRelayCommand(NavigateToRentalsAsync);
            EditVehicleCommand = new AsyncRelayCommand<Vehicle>(EditVehicleAsync);
            DeleteVehicleCommand = new AsyncRelayCommand<Vehicle>(DeleteVehicleAsync);

            if (_authService.CurrentUser != null)
            {
                UserName = _authService.CurrentUser.Name;
            }
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

        public string TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public bool HasVehicles
        {
            get => _hasVehicles;
            set => SetProperty(ref _hasVehicles, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddVehicleCommand { get; }
        public ICommand ViewEarningsCommand { get; }
        public ICommand ViewRentalsCommand { get; }
        public ICommand EditVehicleCommand { get; }
        public ICommand DeleteVehicleCommand { get; }

        public async Task LoadVehiclesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null || _authService.CurrentUser == null)
                {
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
                TotalCount = Vehicles.Count.ToString();

                await UpdateStatisticsAsync();
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
                var b when b.Contains("audi") => "🏎️",
                _ => "🚗"
            };
        }

        public Color GetVehicleBackgroundColor(string brand)
        {
            return brand.ToLower() switch
            {
                var b when b.Contains("toyota") => Color.FromArgb("#E3F2FD"),
                var b when b.Contains("honda") => Color.FromArgb("#FFF3E0"),
                var b when b.Contains("bmw") => Color.FromArgb("#F3E5F5"),
                var b when b.Contains("mercedes") => Color.FromArgb("#E8F5E9"),
                _ => Color.FromArgb("#F5F5F5")
            };
        }

        private async Task NavigateToAddVehicleAsync()
        {
            try
            {
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(new AddVehiclePage());
                }
                else if (Application.Current?.MainPage is Shell shell)
                {
                    if (shell.CurrentPage?.Navigation != null)
                    {
                        await shell.CurrentPage.Navigation.PushAsync(new AddVehiclePage());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando a AddVehicle: {ex.Message}");
            }
        }

        private async Task NavigateToEarningsAsync()
        {
            try
            {
                if (Application.Current?.MainPage is Shell shell && shell.CurrentPage?.Navigation != null)
                {
                    await shell.CurrentPage.Navigation.PushAsync(new OwnerEarningsPage());
                }
                else if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(new OwnerEarningsPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando a Earnings: {ex.Message}");
            }
        }

        private async Task NavigateToRentalsAsync()
        {
            try
            {
                if (Application.Current?.MainPage is Shell shell && shell.CurrentPage?.Navigation != null)
                {
                    await shell.CurrentPage.Navigation.PushAsync(new OwnerRentalsPage());
                }
                else if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(new OwnerRentalsPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando a Rentals: {ex.Message}");
            }
        }

        private async Task EditVehicleAsync(Vehicle? vehicle)
        {
            if (vehicle == null) return;

            try
            {
                var editPage = new EditVehiclePage(vehicle);

                if (Application.Current?.MainPage is Shell shell 
                    && shell.CurrentPage?.Navigation != null)
                {
                    await shell.CurrentPage.Navigation.PushAsync(editPage);
                }
                else if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(editPage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error abriendo editor: {ex.Message}");
            }
        }

        private async Task DeleteVehicleAsync(Vehicle? vehicle)
        {
            if (vehicle == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "⚠️ Eliminar Vehículo",
                $"¿Estás seguro de eliminar {vehicle.DisplayName}?\n\nEsta acción no se puede deshacer.",
                "Sí, eliminar",
                "Cancelar");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                var client = _authService.GetClient();
                if (client == null) return;

                await client.From<Vehicle>()
                    .Where(x => x.Id == vehicle.Id)
                    .Delete();

                Vehicles.Remove(vehicle);
                HasVehicles = Vehicles.Count > 0;
                TotalCount = Vehicles.Count.ToString();

                await Application.Current!.MainPage!.DisplayAlert(
                    "✅ Eliminado",
                    "El vehículo ha sido eliminado",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    ErrorTranslator.TranslateError(ex.Message),
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
