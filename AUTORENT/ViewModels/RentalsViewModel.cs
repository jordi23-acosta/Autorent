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
        private ObservableCollection<RentalWithVehicle> _pendingRentals;
        private ObservableCollection<RentalWithVehicle> _completedRentals;
        private bool _hasRentals;
        private bool _hasActiveRentals;
        private bool _hasUpcomingRentals;
        private bool _hasPendingRentals;
        private bool _hasCompletedRentals;
        private string _selectedFilter = "Todas";
        private int _totalRentals;
        private int _activeCount;
        private int _upcomingCount;
        private int _pendingCount;
        private int _completedCount;

        public RentalsViewModel()
        {
            _authService = AuthService.Instance;
            _activeRentals = new ObservableCollection<RentalWithVehicle>();
            _upcomingRentals = new ObservableCollection<RentalWithVehicle>();
            _pendingRentals = new ObservableCollection<RentalWithVehicle>();
            _completedRentals = new ObservableCollection<RentalWithVehicle>();
            
            Title = "Mis Rentas";
            
            RefreshCommand = new AsyncRelayCommand(LoadRentalsAsync);
            FilterCommand = new RelayCommand<string>(FilterByStatus);
            CancelRentalCommand = new AsyncRelayCommand<RentalWithVehicle>(CancelRentalAsync);
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

        public ObservableCollection<RentalWithVehicle> PendingRentals
        {
            get => _pendingRentals;
            set => SetProperty(ref _pendingRentals, value);
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

        public bool HasActiveRentals
        {
            get => _hasActiveRentals;
            set => SetProperty(ref _hasActiveRentals, value);
        }

        public bool HasUpcomingRentals
        {
            get => _hasUpcomingRentals;
            set => SetProperty(ref _hasUpcomingRentals, value);
        }

        public bool HasPendingRentals
        {
            get => _hasPendingRentals;
            set => SetProperty(ref _hasPendingRentals, value);
        }

        public bool HasCompletedRentals
        {
            get => _hasCompletedRentals;
            set => SetProperty(ref _hasCompletedRentals, value);
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set => SetProperty(ref _selectedFilter, value);
        }

        public int TotalRentals
        {
            get => _totalRentals;
            set => SetProperty(ref _totalRentals, value);
        }

        public int ActiveCount
        {
            get => _activeCount;
            set => SetProperty(ref _activeCount, value);
        }

        public int UpcomingCount
        {
            get => _upcomingCount;
            set => SetProperty(ref _upcomingCount, value);
        }

        public int PendingCount
        {
            get => _pendingCount;
            set => SetProperty(ref _pendingCount, value);
        }

        public int CompletedCount
        {
            get => _completedCount;
            set => SetProperty(ref _completedCount, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand CancelRentalCommand { get; }

        public async Task LoadRentalsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null || _authService.CurrentUser == null) return;

                var userId = _authService.CurrentUser.Id;

                var rentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.RenterId == userId)
                    .Order(x => x.StartDate, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                ActiveRentals.Clear();
                UpcomingRentals.Clear();
                PendingRentals.Clear();
                CompletedRentals.Clear();

                if (rentalsResponse?.Models == null || rentalsResponse.Models.Count == 0)
                {
                    HasRentals = false;
                    UpdateCounts();
                    return;
                }

                // Cargar vehículos asociados
                var vehicleIds = rentalsResponse.Models.Select(r => r.VehicleId).Distinct().ToList();
                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.In, vehicleIds)
                    .Get();

                var vehiclesDict = vehiclesResponse?.Models?.ToDictionary(v => v.Id) 
                    ?? new Dictionary<string, Vehicle>();

                var rentalsWithVehicles = rentalsResponse.Models
                    .Select(r => new RentalWithVehicle
                    {
                        Rental = r,
                        Vehicle = vehiclesDict.ContainsKey(r.VehicleId) ? vehiclesDict[r.VehicleId] : null
                    })
                    .Where(rv => rv.Vehicle != null)
                    .ToList();

                var now = DateTime.Now;

                foreach (var rv in rentalsWithVehicles)
                {
                    var status = rv.Rental.Status;

                    if (status == RentalStatus.Completed || status == RentalStatus.Cancelled)
                    {
                        CompletedRentals.Add(rv);
                    }
                    else if (status == RentalStatus.Pending)
                    {
                        PendingRentals.Add(rv);
                    }
                    else if (rv.Rental.StartDate <= now && rv.Rental.EndDate >= now)
                    {
                        ActiveRentals.Add(rv);
                    }
                    else if (rv.Rental.StartDate > now)
                    {
                        UpcomingRentals.Add(rv);
                    }
                    else
                    {
                        CompletedRentals.Add(rv);
                    }
                }

                UpdateCounts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando rentas: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateCounts()
        {
            ActiveCount = ActiveRentals.Count;
            UpcomingCount = UpcomingRentals.Count;
            PendingCount = PendingRentals.Count;
            CompletedCount = CompletedRentals.Count;
            TotalRentals = ActiveCount + UpcomingCount + PendingCount + CompletedCount;
            
            HasActiveRentals = ActiveCount > 0;
            HasUpcomingRentals = UpcomingCount > 0;
            HasPendingRentals = PendingCount > 0;
            HasCompletedRentals = CompletedCount > 0;
            HasRentals = TotalRentals > 0;
        }

        private void FilterByStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return;
            SelectedFilter = status;
        }

        private async Task CancelRentalAsync(RentalWithVehicle? rv)
        {
            if (rv == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "⚠️ Cancelar Renta",
                $"¿Estás seguro de cancelar la renta de {rv.Vehicle?.DisplayName}?",
                "Sí, cancelar",
                "No");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                var client = _authService.GetClient();
                if (client == null) return;

                await client.From<Rental>()
                    .Where(x => x.Id == rv.Rental.Id)
                    .Set(x => x.StatusString, "cancelada")
                    .Set(x => x.UpdatedAt, DateTime.UtcNow)
                    .Update();

                await LoadRentalsAsync();

                await Application.Current!.MainPage!.DisplayAlert(
                    "✅ Cancelada",
                    "La renta ha sido cancelada",
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

        // Propiedades helper para bindings directos
        public string VehicleName => Vehicle?.DisplayName ?? "Vehículo no disponible";
        public string VehicleBrand => Vehicle?.Brand ?? "";
        public int VehicleSeats => Vehicle?.Seats ?? 0;
        public string VehicleTransmission => Vehicle?.TransmissionDisplay ?? "";
        public string VehicleEmoji => "🚗";
        public string StartDateFormatted => Rental.StartDate.ToString("dd MMM yyyy");
        public string EndDateFormatted => Rental.EndDate.ToString("dd MMM yyyy");
        public string StartTimeFormatted => Rental.StartDate.ToString("HH:mm");
        public string EndTimeFormatted => Rental.EndDate.ToString("HH:mm");
        public string DateRange => $"{Rental.StartDate:dd MMM} - {Rental.EndDate:dd MMM yyyy}";
        public int TotalDays => Math.Max(1, (int)(Rental.EndDate - Rental.StartDate).TotalDays);
        public string TotalPriceFormatted => $"${Rental.TotalPrice:F0}";
        public string PickupLocation => Rental.PickupLocation ?? "Sin ubicación especificada";
        
        public string StatusBadgeText => Rental.Status switch
        {
            RentalStatus.Pending => "⏳ Pendiente",
            RentalStatus.Active => "🟢 Activa",
            RentalStatus.Completed => "✓ Completada",
            RentalStatus.Cancelled => "✕ Cancelada",
            _ => Rental.StatusString
        };

        public Color StatusBadgeColor => Rental.Status switch
        {
            RentalStatus.Pending => Color.FromArgb("#FFF3E0"),
            RentalStatus.Active => Color.FromArgb("#E8F5E9"),
            RentalStatus.Completed => Color.FromArgb("#E3F2FD"),
            RentalStatus.Cancelled => Color.FromArgb("#FFEBEE"),
            _ => Color.FromArgb("#F5F5F5")
        };

        public Color StatusBadgeTextColor => Rental.Status switch
        {
            RentalStatus.Pending => Color.FromArgb("#F57C00"),
            RentalStatus.Active => Color.FromArgb("#388E3C"),
            RentalStatus.Completed => Color.FromArgb("#1976D2"),
            RentalStatus.Cancelled => Color.FromArgb("#D32F2F"),
            _ => Color.FromArgb("#757575")
        };

        public bool CanCancel => Rental.Status == RentalStatus.Pending || Rental.Status == RentalStatus.Active;
    }
}
