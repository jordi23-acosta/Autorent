using AUTORENT.Services;
using AUTORENT.Models;
using System.Collections.ObjectModel;

namespace AUTORENT
{
    public partial class MainPage : ContentPage
    {
        private readonly AuthService _authService;
        private ObservableCollection<Vehicle> _availableVehicles = new();

        public MainPage()
        {
            InitializeComponent();
            _authService = AuthService.Instance;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAvailableVehiclesAsync();
        }

        private async Task LoadAvailableVehiclesAsync()
        {
            try
            {
                var client = _authService.GetClient();
                if (client == null) return;

                // Cargar vehículos disponibles (máximo 10 para la página de inicio)
                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Where(x => x.IsAvailable == true)
                    .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(10)
                    .Get();

                _availableVehicles.Clear();
                if (vehiclesResponse?.Models != null)
                {
                    foreach (var vehicle in vehiclesResponse.Models)
                    {
                        _availableVehicles.Add(vehicle);
                    }
                }

                BuildVehiclesUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando vehículos: {ex.Message}");
            }
        }

        private void BuildVehiclesUI()
        {
            VehiclesContainer.Children.Clear();

            if (_availableVehicles.Count == 0)
            {
                // Mostrar mensaje si no hay vehículos
                VehiclesContainer.Children.Add(new Label
                {
                    Text = "No hay vehículos disponibles en este momento",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#757575"),
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(20)
                });
                return;
            }

            foreach (var vehicle in _availableVehicles)
            {
                VehiclesContainer.Children.Add(CreateVehicleCard(vehicle));
            }
        }

        private Frame CreateVehicleCard(Vehicle vehicle)
        {
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                RowSpacing = 10
            };

            // Fila 0: Información del vehículo
            var vehicleStack = new HorizontalStackLayout { Spacing = 15 };

            var iconFrame = new Frame
            {
                WidthRequest = 70,
                HeightRequest = 70,
                CornerRadius = 10,
                Padding = 0,
                BackgroundColor = Color.FromArgb("#E3F2FD"),
                HasShadow = false,
                Content = new Label
                {
                    Text = GetVehicleEmoji(vehicle.Brand),
                    FontSize = 35,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var infoStack = new VerticalStackLayout
            {
                Spacing = 5,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            infoStack.Children.Add(new Label
            {
                Text = vehicle.DisplayName,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#212121")
            });

            infoStack.Children.Add(new Label
            {
                Text = $"{vehicle.Seats} asientos • {vehicle.TransmissionDisplay}",
                FontSize = 12,
                TextColor = Color.FromArgb("#757575")
            });

            if (!string.IsNullOrEmpty(vehicle.Location))
            {
                infoStack.Children.Add(new Label
                {
                    Text = $"📍 {vehicle.Location}",
                    FontSize = 11,
                    TextColor = Color.FromArgb("#1E88E5")
                });
            }

            vehicleStack.Children.Add(iconFrame);
            vehicleStack.Children.Add(infoStack);

            grid.Children.Add(vehicleStack);
            Grid.SetRow(vehicleStack, 0);

            // Fila 1: Precio
            var priceStack = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.End
            };

            priceStack.Children.Add(new Label
            {
                Text = $"${vehicle.PricePerDay:F0}",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#4CAF50"),
                VerticalOptions = LayoutOptions.Center
            });

            priceStack.Children.Add(new Label
            {
                Text = "/día",
                FontSize = 12,
                TextColor = Color.FromArgb("#757575"),
                VerticalOptions = LayoutOptions.Center
            });

            grid.Children.Add(priceStack);
            Grid.SetRow(priceStack, 1);

            return new Frame
            {
                CornerRadius = 15,
                HasShadow = true,
                BackgroundColor = Colors.White,
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10),
                Content = grid
            };
        }

        private string GetVehicleEmoji(string brand)
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

        private async void OnSearchClicked(object? sender, EventArgs e)
        {
            await DisplayAlert("Búsqueda", "Función de búsqueda en desarrollo", "OK");
        }

        private async void OnRefreshClicked(object? sender, EventArgs e)
        {
            await LoadAvailableVehiclesAsync();
        }
    }
}
