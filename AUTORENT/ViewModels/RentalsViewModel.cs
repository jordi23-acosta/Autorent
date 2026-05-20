using AUTORENT.Services;
using AUTORENT.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class RentalsViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        private ObservableCollection<RentalWithVehicle> _activeRentals;
        private ObservableCollection<RentalWithVehicle> _upcomingRentals;
        private ObservableCollection<RentalWithVehicle> _completedRentals;
        private bool _hasRentals;

        public RentalsViewModel()
        {
            _authService = AuthService.Instance;
            _activeRentals = new ObservableCollection<RentalWithVehicle>();
            _upcomingRentals = new ObservableCollection<RentalWithVehicle>();
            _completedRentals = new ObservableCollection<RentalWithVehicle>();
            
            Title = "Mis Rentas";
            
            RefreshCommand = new AsyncRelayCommand(LoadRentalsAsync);
        }

        public ObservableCollection<RentalWithVehicle> ActiveRentals
        {
            get => _activeRentals;
            set => SetProperty(ref _activeRentals, value);
        }

        public ObservableCollection<RentalWithVehicle> UpcomingRentals
        {
            get => _upcomingRentals;
            set => SetProperty(ref _upcomingRentals, value);
        }

        public ObservableCollection<RentalWithVehicle> CompletedRentals
        {
            get => _completedRentals;
            set => SetProperty(ref _completedRentals, value);
        }

        public bool HasRentals
        {
            get => _hasRentals;
            set => SetProperty(ref _hasRentals, value);
        }

        public ICommand RefreshCommand { get; }

        public async Task LoadRentalsAsync()
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

                // Cargar todas las rentas del usuario (como conductor/renter)
                var rentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.RenterId == userId)
                    .Order(x => x.StartDate, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                if (rentalsResponse?.Models == null || rentalsResponse.Models.Count == 0)
                {
                    HasRentals = false;
                    ActiveRentals.Clear();
                    UpcomingRentals.Clear();
                    CompletedRentals.Clear();
                    return;
                }

                // Cargar los vehículos asociados
                var vehicleIds = rentalsResponse.Models.Select(r => r.VehicleId).Distinct().ToList();
                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.In, vehicleIds)
                    .Get();

                var vehiclesDict = vehiclesResponse?.Models?.ToDictionary(v => v.Id) ?? new Dictionary<string, Vehicle>();

                // Combinar rentas con vehículos
                var rentalsWithVehicles = rentalsResponse.Models
                    .Select(r => new RentalWithVehicle
                    {
                        Rental = r,
                        Vehicle = vehiclesDict.ContainsKey(r.VehicleId) ? vehiclesDict[r.VehicleId] : null
                    })
                    .Where(rv => rv.Vehicle != null)
                    .ToList();

                // Clasificar rentas
                var now = DateTime.Now;
                ActiveRentals.Clear();
                UpcomingRentals.Clear();
                CompletedRentals.Clear();

                foreach (var rv in rentalsWithVehicles)
                {
                    if (rv.Rental.Status == RentalStatus.Completed || rv.Rental.Status == RentalStatus.Cancelled)
                    {
                        CompletedRentals.Add(rv);
                    }
                    else if (rv.Rental.StartDate <= now && rv.Rental.EndDate >= now)
                    {
                        ActiveRentals.Add(rv);
                    }
                    else if (rv.Rental.StartDate > now)
                    {
                        UpcomingRentals.Add(rv);
                    }
                }

                HasRentals = ActiveRentals.Count > 0 || UpcomingRentals.Count > 0 || CompletedRentals.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando rentas: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error", 
                    "No se pudieron cargar las rentas", 
                    "OK");
            }
            finally
            {
                IsBusy = false;
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
    }

    public class RentalWithVehicle
    {
        public Rental Rental { get; set; } = null!;
        public Vehicle? Vehicle { get; set; }
    }
}
