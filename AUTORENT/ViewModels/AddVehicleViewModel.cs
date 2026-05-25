using AUTORENT.Models;
using AUTORENT.Services;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class AddVehicleViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        private string _brand = string.Empty;
        private string _model = string.Empty;
        private string _year = string.Empty;
        private string _plates = string.Empty;
        private string _color = string.Empty;
        private string _price = string.Empty;
        private string _description = string.Empty;
        private string _location = string.Empty;
        private int _seats = 5;
        private bool _isAutomatic = true;
        private VehicleCategory _selectedCategory = VehicleCategory.Economic;
        private int _photoCount = 0;
        private readonly string?[] _photoPaths = new string?[3];
        private string _generalError = string.Empty;
        private bool _hasGeneralError;

        public AddVehicleViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Agregar Vehículo";

            PublishCommand = new AsyncRelayCommand(PublishAsync, CanPublish);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
            IncrementSeatsCommand = new RelayCommand(IncrementSeats);
            DecrementSeatsCommand = new RelayCommand(DecrementSeats);
            SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
            DismissErrorCommand = new RelayCommand(DismissError);
        }

        // Properties
        public string Brand
        {
            get => _brand;
            set
            {
                if (SetProperty(ref _brand, value))
                {
                    OnPropertyChanged(nameof(IsBrandValid));
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                if (SetProperty(ref _model, value))
                {
                    OnPropertyChanged(nameof(IsModelValid));
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Year
        {
            get => _year;
            set
            {
                if (SetProperty(ref _year, value))
                {
                    OnPropertyChanged(nameof(IsYearValid));
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Plates
        {
            get => _plates;
            set
            {
                if (SetProperty(ref _plates, value))
                {
                    OnPropertyChanged(nameof(IsPlatesValid));
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                if (SetProperty(ref _color, value))
                {
                    OnPropertyChanged(nameof(IsColorValid));
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                {
                    OnPropertyChanged(nameof(IsPriceValid));
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public int Seats
        {
            get => _seats;
            set => SetProperty(ref _seats, value);
        }

        public bool IsAutomatic
        {
            get => _isAutomatic;
            set => SetProperty(ref _isAutomatic, value);
        }

        public VehicleCategory SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public int PhotoCount
        {
            get => _photoCount;
            set
            {
                if (SetProperty(ref _photoCount, value))
                {
                    OnPropertyChanged(nameof(PhotoCountText));
                    OnPropertyChanged(nameof(PhotoCountColor));
                    OnPropertyChanged(nameof(PhotoProgress));
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
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

        public string PhotoCountText => $"{PhotoCount} de 3 fotos mínimas";
        
        public Microsoft.Maui.Graphics.Color PhotoCountColor => PhotoCount >= 3 
            ? Microsoft.Maui.Graphics.Color.FromArgb("#4CAF50") 
            : Microsoft.Maui.Graphics.Color.FromArgb("#FF5722");

        public double PhotoProgress => PhotoCount / 3.0;

        // Validation properties for visual feedback
        public bool IsBrandValid => !string.IsNullOrWhiteSpace(_brand) && _brand.Length >= 2;
        public bool IsModelValid => !string.IsNullOrWhiteSpace(_model) && _model.Length >= 1;
        public bool IsYearValid => int.TryParse(_year, out int y) && y >= 1990 && y <= DateTime.Now.Year + 1;
        public bool IsPlatesValid => !string.IsNullOrWhiteSpace(_plates) && _plates.Length >= 5;
        public bool IsColorValid => !string.IsNullOrWhiteSpace(_color) && _color.Length >= 3;
        public bool IsPriceValid => decimal.TryParse(_price, out decimal p) && p > 0;

        public string?[] PhotoPaths => _photoPaths;

        // Commands
        public ICommand PublishCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand IncrementSeatsCommand { get; }
        public ICommand DecrementSeatsCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand DismissErrorCommand { get; }

        private bool CanPublish()
        {
            // Botón siempre habilitado si no está ocupado
            // La validación se hace dentro de PublishAsync con mensajes claros
            return !IsBusy;
        }

        private async Task PublishAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                GeneralError = string.Empty;

                // Validaciones manuales con mensajes claros
                if (!IsBrandValid)
                {
                    GeneralError = "Ingresa la marca del vehículo";
                    return;
                }

                if (!IsModelValid)
                {
                    GeneralError = "Ingresa el modelo del vehículo";
                    return;
                }

                if (!int.TryParse(Year, out int year) || year < 1990 || year > DateTime.Now.Year + 1)
                {
                    GeneralError = $"El año debe estar entre 1990 y {DateTime.Now.Year + 1}";
                    return;
                }

                if (!IsColorValid)
                {
                    GeneralError = "Ingresa el color del vehículo";
                    return;
                }

                if (!IsPlatesValid)
                {
                    GeneralError = "Ingresa las placas (mínimo 5 caracteres)";
                    return;
                }

                if (!decimal.TryParse(Price, out decimal price) || price <= 0)
                {
                    GeneralError = "Ingresa un precio válido mayor a $0";
                    return;
                }

                var client = _authService.GetClient();
                if (client == null)
                {
                    GeneralError = "No se pudo conectar con el servidor";
                    return;
                }

                if (_authService.CurrentUser == null)
                {
                    GeneralError = "Sesión inválida. Cierra y vuelve a iniciar sesión";
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] OwnerId: {_authService.CurrentUser.Id}");
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] Marca: {Brand}, Modelo: {Model}, Año: {year}, Precio: {price}");

                var vehicle = new Vehicle
                {
                    Brand = Brand.Trim(),
                    Model = Model.Trim(),
                    Year = year,
                    Plate = Plates.Trim().ToUpper(),
                    Color = Color.Trim(),
                    Seats = Seats,
                    Transmission = IsAutomatic ? "automatico" : "manual",
                    FuelType = "gasolina",
                    PricePerDay = price,
                    Description = Description?.Trim() ?? "",
                    Location = Location?.Trim() ?? "",
                    IsAvailable = true,
                    // No guardamos rutas locales como URL (causa errores)
                    ImageUrl = null,
                    OwnerId = _authService.CurrentUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] Insertando vehículo en BD...");
                
                var response = await client.From<Vehicle>().Insert(vehicle);
                
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] ✅ Vehículo guardado. Modelos: {response?.Models?.Count ?? 0}");

                await Application.Current!.MainPage!.DisplayAlert(
                    "🎉 ¡Publicado!",
                    $"Tu {vehicle.Brand} {vehicle.Model} {vehicle.Year} ya está disponible para rentar.",
                    "OK");

                await NavigateBackAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] ❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] Stack: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] Inner: {ex.InnerException.Message}");
                }
                GeneralError = ErrorTranslator.TranslateError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            // Solo pedir confirmación si hay datos ingresados
            bool hasData = !string.IsNullOrWhiteSpace(Brand) || 
                          !string.IsNullOrWhiteSpace(Model) ||
                          PhotoCount > 0;

            if (hasData)
            {
                bool answer = await Application.Current!.MainPage!.DisplayAlert(
                    "¿Cancelar?",
                    "Se perderán los datos ingresados", 
                    "Sí, cancelar", 
                    "Seguir editando");
                
                if (!answer) return;
            }

            await NavigateBackAsync();
        }

        private async Task NavigateBackAsync()
        {
            try
            {
                if (Application.Current?.MainPage is NavigationPage navPage 
                    && navPage.Navigation.NavigationStack.Count > 1)
                {
                    await navPage.PopAsync();
                }
                else if (Application.Current?.MainPage is Shell shell)
                {
                    if (shell.CurrentPage?.Navigation != null 
                        && shell.CurrentPage.Navigation.NavigationStack.Count > 1)
                    {
                        await shell.CurrentPage.Navigation.PopAsync();
                    }
                    else
                    {
                        await shell.GoToAsync("..");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando atrás: {ex.Message}");
            }
        }

        private void IncrementSeats()
        {
            if (Seats < 12) Seats++;
        }

        private void DecrementSeats()
        {
            if (Seats > 2) Seats--;
        }

        public void SelectCategory(string? category)
        {
            if (string.IsNullOrEmpty(category)) return;

            SelectedCategory = category switch
            {
                "Economic" => VehicleCategory.Economic,
                "SUV" => VehicleCategory.SUV,
                "Luxury" => VehicleCategory.Luxury,
                "Van" => VehicleCategory.Van,
                _ => VehicleCategory.Economic
            };
        }

        public void SetPhotoPath(int slot, string? path)
        {
            if (slot < 0 || slot >= 3) return;

            bool wasEmpty = _photoPaths[slot] == null;
            _photoPaths[slot] = path;

            if (wasEmpty && path != null) PhotoCount++;
            else if (!wasEmpty && path == null) PhotoCount--;
        }

        private void DismissError()
        {
            GeneralError = string.Empty;
        }
    }
}
