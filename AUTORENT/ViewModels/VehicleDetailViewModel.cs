using AUTORENT.Models;
using AUTORENT.Services;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class VehicleDetailViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private Vehicle? _vehicle;
        private Profile? _owner;
        
        private DateTime _startDate = DateTime.Today.AddDays(1);
        private DateTime _endDate = DateTime.Today.AddDays(3);
        private DateTime _minDate = DateTime.Today;
        private string _pickupLocation = string.Empty;
        private string _notes = string.Empty;
        private string _generalError = string.Empty;
        private bool _hasGeneralError;
        private bool _showSuccess;
        private string _paymentMethod = "efectivo";

        public VehicleDetailViewModel(Vehicle vehicle)
        {
            _authService = AuthService.Instance;
            _vehicle = vehicle;
            _pickupLocation = vehicle.Location ?? string.Empty;

            Title = vehicle.DisplayName;

            RequestRentalCommand = new AsyncRelayCommand(RequestRentalAsync, CanRequestRental);
            DismissErrorCommand = new RelayCommand(DismissError);
            SelectPaymentMethodCommand = new RelayCommand<string>(SelectPaymentMethod);

            _ = LoadOwnerInfoAsync();
        }

        // Vehicle properties (read-only from binding)
        public Vehicle? Vehicle
        {
            get => _vehicle;
            set
            {
                if (SetProperty(ref _vehicle, value))
                {
                    OnPropertyChanged(nameof(VehicleDisplayName));
                    OnPropertyChanged(nameof(VehicleBrand));
                    OnPropertyChanged(nameof(VehicleModel));
                    OnPropertyChanged(nameof(VehicleYear));
                    OnPropertyChanged(nameof(VehicleSeats));
                    OnPropertyChanged(nameof(VehicleTransmission));
                    OnPropertyChanged(nameof(VehicleColor));
                    OnPropertyChanged(nameof(VehiclePricePerDay));
                    OnPropertyChanged(nameof(VehiclePriceFormatted));
                    OnPropertyChanged(nameof(VehicleDescription));
                    OnPropertyChanged(nameof(VehicleLocation));
                    OnPropertyChanged(nameof(VehicleEmoji));
                    OnPropertyChanged(nameof(HasDescription));
                    OnPropertyChanged(nameof(TotalDays));
                    OnPropertyChanged(nameof(TotalPrice));
                    OnPropertyChanged(nameof(TotalPriceFormatted));
                }
            }
        }

        public string VehicleDisplayName => Vehicle?.DisplayName ?? "";
        public string VehicleBrand => Vehicle?.Brand ?? "";
        public string VehicleModel => Vehicle?.Model ?? "";
        public string VehicleYear => Vehicle?.Year.ToString() ?? "";
        public int VehicleSeats => Vehicle?.Seats ?? 0;
        public string VehicleTransmission => Vehicle?.TransmissionDisplay ?? "";
        public string VehicleColor => Vehicle?.Color ?? "Sin especificar";
        public decimal VehiclePricePerDay => Vehicle?.PricePerDay ?? 0;
        public string VehiclePriceFormatted => $"${VehiclePricePerDay:F0}";
        public string VehicleDescription => Vehicle?.Description ?? "";
        public string VehicleLocation => Vehicle?.Location ?? "Sin ubicación";
        public bool HasDescription => !string.IsNullOrWhiteSpace(VehicleDescription);

        public string VehicleEmoji => Vehicle?.Brand.ToLower() switch
        {
            var b when b != null && b.Contains("toyota") => "🚙",
            var b when b != null && b.Contains("honda") => "🚐",
            var b when b != null && b.Contains("bmw") => "🏎️",
            var b when b != null && b.Contains("mercedes") => "🚗",
            var b when b != null && b.Contains("ford") => "🚙",
            var b when b != null && b.Contains("chevrolet") => "🚙",
            var b when b != null && b.Contains("audi") => "🏎️",
            _ => "🚗"
        };

        // Owner info
        public Profile? Owner
        {
            get => _owner;
            set
            {
                if (SetProperty(ref _owner, value))
                {
                    OnPropertyChanged(nameof(OwnerName));
                    OnPropertyChanged(nameof(OwnerInitials));
                }
            }
        }

        public string OwnerName => _owner?.FullName ?? "Propietario";
        public string OwnerInitials
        {
            get
            {
                var name = OwnerName.Trim();
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

        // Rental dates
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    if (_endDate <= _startDate)
                    {
                        EndDate = _startDate.AddDays(1);
                    }
                    OnPropertyChanged(nameof(TotalDays));
                    OnPropertyChanged(nameof(TotalPrice));
                    OnPropertyChanged(nameof(TotalPriceFormatted));
                    OnPropertyChanged(nameof(StartDateFormatted));
                    ((AsyncRelayCommand)RequestRentalCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    OnPropertyChanged(nameof(TotalDays));
                    OnPropertyChanged(nameof(TotalPrice));
                    OnPropertyChanged(nameof(TotalPriceFormatted));
                    OnPropertyChanged(nameof(EndDateFormatted));
                    ((AsyncRelayCommand)RequestRentalCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public DateTime MinDate
        {
            get => _minDate;
            set => SetProperty(ref _minDate, value);
        }

        public DateTime MinEndDate => StartDate.AddDays(1);

        public string StartDateFormatted => StartDate.ToString("dd MMM yyyy");
        public string EndDateFormatted => EndDate.ToString("dd MMM yyyy");

        public int TotalDays => Math.Max(1, (int)(EndDate - StartDate).TotalDays);

        public decimal TotalPrice => VehiclePricePerDay * TotalDays;
        public string TotalPriceFormatted => $"${TotalPrice:N0}";

        public string PickupLocation
        {
            get => _pickupLocation;
            set => SetProperty(ref _pickupLocation, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public string GeneralError
        {
            get => _generalError;
            set
            {
                if (SetProperty(ref _generalError, value))
                    HasGeneralError = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasGeneralError
        {
            get => _hasGeneralError;
            set => SetProperty(ref _hasGeneralError, value);
        }

        public bool ShowSuccess
        {
            get => _showSuccess;
            set => SetProperty(ref _showSuccess, value);
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set
            {
                if (SetProperty(ref _paymentMethod, value))
                {
                    OnPropertyChanged(nameof(IsPaymentCash));
                    OnPropertyChanged(nameof(IsPaymentTransfer));
                    OnPropertyChanged(nameof(PaymentMethodLabel));
                }
            }
        }

        public bool IsPaymentCash => _paymentMethod == "efectivo";
        public bool IsPaymentTransfer => _paymentMethod == "transferencia";

        public string PaymentMethodLabel => _paymentMethod switch
        {
            "efectivo" => "💵 Efectivo al recibir el auto",
            "transferencia" => "🏦 Transferencia bancaria",
            _ => "Selecciona método de pago"
        };

        public ICommand RequestRentalCommand { get; }
        public ICommand DismissErrorCommand { get; }
        public ICommand SelectPaymentMethodCommand { get; }

        private bool CanRequestRental()
        {
            return Vehicle != null && 
                   StartDate >= DateTime.Today &&
                   EndDate > StartDate &&
                   !IsBusy;
        }

        private async Task LoadOwnerInfoAsync()
        {
            if (Vehicle == null) return;

            try
            {
                var client = _authService.GetClient();
                if (client == null) return;

                var response = await client
                    .From<Profile>()
                    .Where(x => x.Id == Vehicle.OwnerId)
                    .Single();

                if (response != null)
                {
                    Owner = response;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando dueño: {ex.Message}");
            }
        }

        private async Task RequestRentalAsync()
        {
            if (Vehicle == null || IsBusy) return;

            // Validar que el usuario está logeado
            var currentUser = _authService.CurrentUser;
            if (currentUser == null)
            {
                GeneralError = "Debes iniciar sesión para rentar";
                return;
            }

            // Validar que no es el dueño del auto
            if (currentUser.Id == Vehicle.OwnerId)
            {
                GeneralError = "No puedes rentar tu propio vehículo";
                return;
            }

            // Validar fechas
            if (StartDate < DateTime.Today)
            {
                GeneralError = "La fecha de inicio no puede ser en el pasado";
                return;
            }

            if (EndDate <= StartDate)
            {
                GeneralError = "La fecha de devolución debe ser posterior a la de recogida";
                return;
            }

            // Confirmación
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "🚗 Confirmar Renta",
                $"Renta del {Vehicle.DisplayName}\n\n" +
                $"📅 Desde: {StartDateFormatted}\n" +
                $"📅 Hasta: {EndDateFormatted}\n" +
                $"⏱️ {TotalDays} día(s)\n\n" +
                $"💰 Total: {TotalPriceFormatted}\n" +
                $"{PaymentMethodLabel}\n\n" +
                $"¿Confirmas tu solicitud?",
                "Sí, solicitar",
                "Cancelar");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                GeneralError = string.Empty;

                var client = _authService.GetClient();
                if (client == null)
                {
                    GeneralError = "No se pudo conectar con el servidor";
                    return;
                }

                var rental = new Rental
                {
                    VehicleId = Vehicle.Id,
                    RenterId = currentUser.Id,
                    OwnerId = Vehicle.OwnerId,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    TotalPrice = TotalPrice,
                    StatusString = "pendiente",
                    PaymentMethod = PaymentMethod,
                    PickupLocation = PickupLocation?.Trim() ?? "",
                    Notes = Notes?.Trim() ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                System.Diagnostics.Debug.WriteLine($"[RENTAL] Creando solicitud para {Vehicle.DisplayName}");

                await client.From<Rental>().Insert(rental);

                System.Diagnostics.Debug.WriteLine($"[RENTAL] ✅ Solicitud creada exitosamente");

                ShowSuccess = true;

                string paymentInfo = PaymentMethod == "efectivo"
                    ? "💵 Pagarás en efectivo al recibir el auto"
                    : "🏦 El propietario te enviará los datos bancarios al aceptar";

                await Application.Current!.MainPage!.DisplayAlert(
                    "🎉 ¡Solicitud Enviada!",
                    $"Tu solicitud de renta ha sido enviada al propietario.\n\n" +
                    $"{paymentInfo}\n\n" +
                    $"Recibirás una notificación cuando sea aprobada.\n\n" +
                    $"Puedes ver el estado en 'Mis Rentas'.",
                    "OK");

                await NavigateBackAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RENTAL] ❌ Error: {ex.Message}");
                GeneralError = ErrorTranslator.TranslateError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task NavigateBackAsync()
        {
            try
            {
                if (Application.Current?.MainPage is NavigationPage navPage 
                    && navPage.Navigation.NavigationStack.Count > 1)
                {
                    await navPage.PopAsync();
                }
                else if (Application.Current?.MainPage is Shell shell
                    && shell.CurrentPage?.Navigation != null
                    && shell.CurrentPage.Navigation.NavigationStack.Count > 1)
                {
                    await shell.CurrentPage.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando atrás: {ex.Message}");
            }
        }

        private void DismissError()
        {
            GeneralError = string.Empty;
        }

        private void SelectPaymentMethod(string? method)
        {
            if (string.IsNullOrEmpty(method)) return;
            PaymentMethod = method;
        }
    }
}
