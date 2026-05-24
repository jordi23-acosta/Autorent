using AUTORENT.Models;
using AUTORENT.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class OwnerRentalsViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private ObservableCollection<RentalRequest> _pendingRequests;
        private ObservableCollection<RentalRequest> _activeRentals;
        private ObservableCollection<RentalRequest> _completedRentals;

        private bool _hasPendingRequests;
        private bool _hasActiveRentals;
        private bool _hasCompletedRentals;
        private bool _hasAnyData;
        private int _pendingCount;
        private int _activeCount;
        private int _completedCount;

        public OwnerRentalsViewModel()
        {
            _authService = AuthService.Instance;
            _pendingRequests = new ObservableCollection<RentalRequest>();
            _activeRentals = new ObservableCollection<RentalRequest>();
            _completedRentals = new ObservableCollection<RentalRequest>();

            Title = "Solicitudes";

            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
            AcceptRequestCommand = new AsyncRelayCommand<RentalRequest>(AcceptRequestAsync);
            RejectRequestCommand = new AsyncRelayCommand<RentalRequest>(RejectRequestAsync);
            ViewDetailsCommand = new AsyncRelayCommand<RentalRequest>(ViewDetailsAsync);
        }

        public ObservableCollection<RentalRequest> PendingRequests
        {
            get => _pendingRequests;
            set => SetProperty(ref _pendingRequests, value);
        }

        public ObservableCollection<RentalRequest> ActiveRentals
        {
            get => _activeRentals;
            set => SetProperty(ref _activeRentals, value);
        }

        public ObservableCollection<RentalRequest> CompletedRentals
        {
            get => _completedRentals;
            set => SetProperty(ref _completedRentals, value);
        }

        public bool HasPendingRequests
        {
            get => _hasPendingRequests;
            set => SetProperty(ref _hasPendingRequests, value);
        }

        public bool HasActiveRentals
        {
            get => _hasActiveRentals;
            set => SetProperty(ref _hasActiveRentals, value);
        }

        public bool HasCompletedRentals
        {
            get => _hasCompletedRentals;
            set => SetProperty(ref _hasCompletedRentals, value);
        }

        public bool HasAnyData
        {
            get => _hasAnyData;
            set => SetProperty(ref _hasAnyData, value);
        }

        public int PendingCount
        {
            get => _pendingCount;
            set
            {
                if (SetProperty(ref _pendingCount, value))
                    OnPropertyChanged(nameof(PendingCountText));
            }
        }

        public int ActiveCount
        {
            get => _activeCount;
            set => SetProperty(ref _activeCount, value);
        }

        public int CompletedCount
        {
            get => _completedCount;
            set => SetProperty(ref _completedCount, value);
        }

        public string PendingCountText => PendingCount == 1 ? "1 nueva" : $"{PendingCount} nuevas";

        public ICommand RefreshCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand RejectRequestCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null || _authService.CurrentUser == null) return;

                var userId = _authService.CurrentUser.Id;

                // Cargar todas las rentas donde el usuario es propietario
                var rentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.OwnerId == userId)
                    .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                PendingRequests.Clear();
                ActiveRentals.Clear();
                CompletedRentals.Clear();

                if (rentalsResponse?.Models == null || rentalsResponse.Models.Count == 0)
                {
                    UpdateCounts();
                    return;
                }

                // Cargar vehículos asociados
                var vehicleIds = rentalsResponse.Models.Select(r => r.VehicleId).Distinct().ToList();
                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.In, vehicleIds)
                    .Get();
                var vehicles = vehiclesResponse?.Models?.ToDictionary(v => v.Id) 
                    ?? new Dictionary<string, Vehicle>();

                // Cargar perfiles de los renters
                var renterIds = rentalsResponse.Models.Select(r => r.RenterId).Distinct().ToList();
                var profilesResponse = await client
                    .From<Profile>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.In, renterIds)
                    .Get();
                var profiles = profilesResponse?.Models?.ToDictionary(p => p.Id) 
                    ?? new Dictionary<string, Profile>();

                foreach (var rental in rentalsResponse.Models)
                {
                    var vehicle = vehicles.GetValueOrDefault(rental.VehicleId);
                    var renter = profiles.GetValueOrDefault(rental.RenterId);

                    var request = new RentalRequest
                    {
                        Rental = rental,
                        Vehicle = vehicle,
                        Renter = renter
                    };

                    var status = rental.StatusString;

                    if (status == "pendiente")
                    {
                        PendingRequests.Add(request);
                    }
                    else if (status == "activa" || status == "confirmada")
                    {
                        ActiveRentals.Add(request);
                    }
                    else
                    {
                        CompletedRentals.Add(request);
                    }
                }

                UpdateCounts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OWNER RENTALS] Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateCounts()
        {
            PendingCount = PendingRequests.Count;
            ActiveCount = ActiveRentals.Count;
            CompletedCount = CompletedRentals.Count;

            HasPendingRequests = PendingCount > 0;
            HasActiveRentals = ActiveCount > 0;
            HasCompletedRentals = CompletedCount > 0;
            HasAnyData = HasPendingRequests || HasActiveRentals || HasCompletedRentals;
        }

        private async Task AcceptRequestAsync(RentalRequest? request)
        {
            if (request == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "✓ Aceptar Solicitud",
                $"¿Confirmas que aceptas la renta de {request.RenterName} para {request.VehicleName}?",
                "Sí, aceptar",
                "Cancelar");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                var client = _authService.GetClient();
                if (client == null) return;

                await client.From<Rental>()
                    .Where(x => x.Id == request.Rental.Id)
                    .Set(x => x.StatusString, "confirmada")
                    .Set(x => x.UpdatedAt, DateTime.UtcNow)
                    .Update();

                await Application.Current!.MainPage!.DisplayAlert(
                    "🎉 ¡Confirmado!",
                    "La solicitud ha sido aceptada exitosamente",
                    "OK");

                await LoadDataAsync();
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

        private async Task RejectRequestAsync(RentalRequest? request)
        {
            if (request == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "✕ Rechazar Solicitud",
                $"¿Estás seguro de rechazar la renta de {request.RenterName}?",
                "Sí, rechazar",
                "Cancelar");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                var client = _authService.GetClient();
                if (client == null) return;

                await client.From<Rental>()
                    .Where(x => x.Id == request.Rental.Id)
                    .Set(x => x.StatusString, "cancelada")
                    .Set(x => x.UpdatedAt, DateTime.UtcNow)
                    .Update();

                await Application.Current!.MainPage!.DisplayAlert(
                    "Rechazada",
                    "La solicitud ha sido rechazada",
                    "OK");

                await LoadDataAsync();
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

        private async Task ViewDetailsAsync(RentalRequest? request)
        {
            if (request == null) return;

            await Application.Current!.MainPage!.DisplayAlert(
                "📋 Detalles de Renta",
                $"Cliente: {request.RenterName}\n" +
                $"Email: {request.RenterEmail}\n" +
                $"Teléfono: {request.RenterPhone}\n\n" +
                $"Vehículo: {request.VehicleName}\n" +
                $"Fechas: {request.DateRangeText}\n" +
                $"Total: {request.TotalPriceFormatted}",
                "OK");
        }
    }

    public class RentalRequest
    {
        public Rental Rental { get; set; } = null!;
        public Vehicle? Vehicle { get; set; }
        public Profile? Renter { get; set; }

        public string RenterName => Renter?.FullName ?? "Usuario";
        public string RenterEmail => Renter?.Email ?? "Sin email";
        public string RenterPhone => Renter?.Phone ?? "Sin teléfono";
        public string RenterInitials
        {
            get
            {
                var name = RenterName.Trim();
                if (string.IsNullOrEmpty(name)) return "?";
                
                var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    return $"{parts[0][0]}{parts[1][0]}".ToUpper();
                if (parts.Length == 1 && parts[0].Length > 0)
                    return parts[0].Length >= 2 
                        ? parts[0].Substring(0, 2).ToUpper() 
                        : parts[0][0].ToString().ToUpper();
                return "?";
            }
        }

        public string VehicleName => Vehicle?.DisplayName ?? "Vehículo no disponible";
        public string VehicleEmoji => Vehicle?.Brand.ToLower() switch
        {
            var b when b != null && b.Contains("toyota") => "🚙",
            var b when b != null && b.Contains("honda") => "🚐",
            var b when b != null && b.Contains("bmw") => "🏎️",
            var b when b != null && b.Contains("mercedes") => "🚗",
            _ => "🚗"
        };

        public string StartDateFormatted => Rental.StartDate.ToString("dd MMM");
        public string EndDateFormatted => Rental.EndDate.ToString("dd MMM yyyy");
        public int TotalDays => Math.Max(1, (int)(Rental.EndDate - Rental.StartDate).TotalDays);
        public string DateRangeText => $"{Rental.StartDate:dd MMM} - {Rental.EndDate:dd MMM yyyy} ({TotalDays} días)";
        public string TotalPriceFormatted => $"${Rental.TotalPrice:F0}";

        public Color AvatarColor
        {
            get
            {
                var hash = RenterName.GetHashCode();
                var colors = new[]
                {
                    Color.FromArgb("#1E88E5"),
                    Color.FromArgb("#9C27B0"),
                    Color.FromArgb("#FF9800"),
                    Color.FromArgb("#43A047"),
                    Color.FromArgb("#E91E63"),
                    Color.FromArgb("#00BCD4"),
                    Color.FromArgb("#FF5722")
                };
                return colors[Math.Abs(hash) % colors.Length];
            }
        }

        public bool IsNewUser => true; // Placeholder - se podría calcular basado en fecha de registro
    }
}
