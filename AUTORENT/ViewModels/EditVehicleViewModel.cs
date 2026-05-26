using AUTORENT.Models;
using AUTORENT.Services;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class EditVehicleViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly Vehicle _originalVehicle;

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
        private bool _isAvailable = true;
        private VehicleCategory _selectedCategory = VehicleCategory.Economic;
        private string _generalError = string.Empty;
        private bool _hasGeneralError;

        public EditVehicleViewModel(Vehicle vehicle)
        {
            _authService = AuthService.Instance;
            _originalVehicle = vehicle;
            Title = $"Editar {vehicle.Brand} {vehicle.Model}";

            // Cargar datos existentes
            LoadVehicleData();

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteAsync);
            IncrementSeatsCommand = new RelayCommand(IncrementSeats);
            DecrementSeatsCommand = new RelayCommand(DecrementSeats);
            SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
            DismissErrorCommand = new RelayCommand(DismissError);
        }

        private void LoadVehicleData()
        {
            Brand = _originalVehicle.Brand;
            Model = _originalVehicle.Model;
            Year = _originalVehicle.Year.ToString();
            Plates = _originalVehicle.Plate ?? string.Empty;
            Color = _originalVehicle.Color ?? string.Empty;
            Price = _originalVehicle.PricePerDay.ToString("F0");
            Description = _originalVehicle.Description ?? string.Empty;
            Location = _originalVehicle.Location ?? string.Empty;
            Seats = _originalVehicle.Seats;
            IsAutomatic = _originalVehicle.IsAutomatic;
            IsAvailable = _originalVehicle.IsAvailable;
        }

        // Properties
        public string Brand
        {
            get => _brand;
            set
            {
                if (SetProperty(ref _brand, value))
                    OnPropertyChanged(nameof(IsBrandValid));
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                if (SetProperty(ref _model, value))
                    OnPropertyChanged(nameof(IsModelValid));
            }
        }

        public string Year
        {
            get => _year;
            set
            {
                if (SetProperty(ref _year, value))
                    OnPropertyChanged(nameof(IsYearValid));
            }
        }

        public string Plates
        {
            get => _plates;
            set
            {
                if (SetProperty(ref _plates, value))
                    OnPropertyChanged(nameof(IsPlatesValid));
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                if (SetProperty(ref _color, value))
                    OnPropertyChanged(nameof(IsColorValid));
            }
        }

        public string Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                    OnPropertyChanged(nameof(IsPriceValid));
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

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        public VehicleCategory SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
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

        // Validation
        public bool IsBrandValid => !string.IsNullOrWhiteSpace(_brand) && _brand.Length >= 2;
        public bool IsModelValid => !string.IsNullOrWhiteSpace(_model) && _model.Length >= 1;
        public bool IsYearValid => int.TryParse(_year, out int y) && y >= 1990 && y <= DateTime.Now.Year + 1;
        public bool IsPlatesValid => !string.IsNullOrWhiteSpace(_plates) && _plates.Length >= 5;
        public bool IsColorValid => !string.IsNullOrWhiteSpace(_color) && _color.Length >= 3;
        public bool IsPriceValid => decimal.TryParse(_price, out decimal p) && p > 0;

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand IncrementSeatsCommand { get; }
        public ICommand DecrementSeatsCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand DismissErrorCommand { get; }

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                GeneralError = string.Empty;

                // Validaciones
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

                System.Diagnostics.Debug.WriteLine($"[EDIT VEHICLE] Actualizando vehículo {_originalVehicle.Id}");

                await client.From<Vehicle>()
                    .Where(x => x.Id == _originalVehicle.Id)
                    .Set(x => x.Brand, Brand.Trim())
                    .Set(x => x.Model, Model.Trim())
                    .Set(x => x.Year, year)
                    .Set(x => x.Plate, Plates.Trim().ToUpper())
                    .Set(x => x.Color, Color.Trim())
                    .Set(x => x.Seats, Seats)
                    .Set(x => x.Transmission, IsAutomatic ? "automatico" : "manual")
                    .Set(x => x.PricePerDay, price)
                    .Set(x => x.Description, Description?.Trim() ?? "")
                    .Set(x => x.Location, Location?.Trim() ?? "")
                    .Set(x => x.IsAvailable, IsAvailable)
                    .Set(x => x.UpdatedAt, DateTime.UtcNow)
                    .Update();

                System.Diagnostics.Debug.WriteLine($"[EDIT VEHICLE] ✅ Vehículo actualizado");

                await Application.Current!.MainPage!.DisplayAlert(
                    "✅ Actualizado",
                    $"Tu {Brand} {Model} {year} ha sido actualizado correctamente",
                    "OK");

                await NavigateBackAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EDIT VEHICLE] ❌ Error: {ex.Message}");
                GeneralError = ErrorTranslator.TranslateError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteAsync()
        {
            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "⚠️ Eliminar Vehículo",
                $"¿Estás seguro de eliminar {_originalVehicle.DisplayName}?\n\nEsta acción no se puede deshacer.",
                "Sí, eliminar",
                "Cancelar");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                var client = _authService.GetClient();
                if (client == null) return;

                await client.From<Vehicle>()
                    .Where(x => x.Id == _originalVehicle.Id)
                    .Delete();

                await Application.Current!.MainPage!.DisplayAlert(
                    "✅ Eliminado",
                    "El vehículo ha sido eliminado",
                    "OK");

                await NavigateBackAsync();
            }
            catch (Exception ex)
            {
                GeneralError = ErrorTranslator.TranslateError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            await NavigateBackAsync();
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

        private void DismissError()
        {
            GeneralError = string.Empty;
        }
    }
}
