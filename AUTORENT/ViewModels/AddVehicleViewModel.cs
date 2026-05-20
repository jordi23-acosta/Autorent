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

        public AddVehicleViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Agregar Vehículo";

            PublishCommand = new AsyncRelayCommand(PublishAsync, CanPublish);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
            IncrementSeatsCommand = new RelayCommand(IncrementSeats);
            DecrementSeatsCommand = new RelayCommand(DecrementSeats);
            SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
        }

        // Properties
        public string Brand
        {
            get => _brand;
            set
            {
                if (SetProperty(ref _brand, value))
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                if (SetProperty(ref _model, value))
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
            }
        }

        public string Year
        {
            get => _year;
            set
            {
                if (SetProperty(ref _year, value))
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
            }
        }

        public string Plates
        {
            get => _plates;
            set
            {
                if (SetProperty(ref _plates, value))
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                if (SetProperty(ref _color, value))
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
            }
        }

        public string Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
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
                    ((AsyncRelayCommand)PublishCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string PhotoCountText => $"{PhotoCount} de 3 fotos mínimas";
        
        public Microsoft.Maui.Graphics.Color PhotoCountColor => PhotoCount >= 3 
            ? Microsoft.Maui.Graphics.Color.FromArgb("#4CAF50") 
            : Microsoft.Maui.Graphics.Color.FromArgb("#FF5722");

        public string?[] PhotoPaths => _photoPaths;

        // Commands
        public ICommand PublishCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand IncrementSeatsCommand { get; }
        public ICommand DecrementSeatsCommand { get; }
        public ICommand SelectCategoryCommand { get; }

        private bool CanPublish()
        {
            return !string.IsNullOrWhiteSpace(Brand) &&
                   !string.IsNullOrWhiteSpace(Model) &&
                   !string.IsNullOrWhiteSpace(Year) &&
                   !string.IsNullOrWhiteSpace(Plates) &&
                   !string.IsNullOrWhiteSpace(Color) &&
                   !string.IsNullOrWhiteSpace(Price) &&
                   PhotoCount >= 1 &&
                   !IsBusy;
        }

        private async Task PublishAsync()
        {
            if (IsBusy) return;

            // Validaciones adicionales
            if (!int.TryParse(Year, out int year) || year < 1990 || year > DateTime.Now.Year + 1)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Año inválido", 
                    "Ingresa un año entre 1990 y el año actual", 
                    "OK");
                return;
            }

            if (!decimal.TryParse(Price, out decimal price) || price <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Precio inválido", 
                    "Ingresa un precio mayor a $0", 
                    "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error", 
                        "No se pudo conectar con el servidor", 
                        "OK");
                    return;
                }

                var vehicle = new Vehicle
                {
                    Brand = Brand.Trim(),
                    Model = Model.Trim(),
                    Year = year,
                    Plate = Plates.Trim().ToUpper(),
                    Color = Color.Trim(),
                    Seats = Seats,
                    Transmission = IsAutomatic ? "automatico" : "manual",
                    PricePerDay = price,
                    Description = Description?.Trim() ?? "",
                    Location = Location?.Trim() ?? "",
                    IsAvailable = true,
                    ImageUrl = _photoPaths[0],
                    OwnerId = _authService.CurrentUser!.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] Guardando vehículo: {vehicle.Brand} {vehicle.Model}");
                
                // Guardar en Supabase
                await client.From<Vehicle>().Insert(vehicle);
                
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] ✅ Vehículo guardado exitosamente");

                await Application.Current!.MainPage!.DisplayAlert(
                    "¡Publicado! 🎉",
                    $"Tu {vehicle.Brand} {vehicle.Model} {vehicle.Year} ya está disponible para rentar.",
                    "Ver mis autos");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] ❌ Error: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error", 
                    $"No se pudo publicar el vehículo: {ex.Message}", 
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            bool answer = await Application.Current!.MainPage!.DisplayAlert(
                "¿Cancelar?",
                "Se perderán los datos ingresados", 
                "Sí, cancelar", 
                "Seguir editando");
            
            if (answer)
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        private void IncrementSeats()
        {
            if (Seats < 12)
            {
                Seats++;
            }
        }

        private void DecrementSeats()
        {
            if (Seats > 2)
            {
                Seats--;
            }
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

            if (wasEmpty && path != null)
            {
                PhotoCount++;
            }
            else if (!wasEmpty && path == null)
            {
                PhotoCount--;
            }
        }
    }
}
