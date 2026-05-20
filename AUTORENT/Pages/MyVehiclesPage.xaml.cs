using AUTORENT.Services;
using AUTORENT.Models;
using System.Collections.ObjectModel;

namespace AUTORENT.Pages
{
    public partial class MyVehiclesPage : ContentPage
    {
        private readonly AuthService _authService;
        private ObservableCollection<Vehicle> _vehicles = new();

        public MyVehiclesPage()
        {
            InitializeComponent();
            _authService = AuthService.Instance;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadVehiclesAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            try
            {
                var client = _authService.GetClient();
                if (client == null || _authService.CurrentUser == null)
                {
                    await DisplayAlert("Error", "No has iniciado sesión", "OK");
                    return;
                }

                var userId = _authService.CurrentUser.Id;

                // Cargar vehículos del propietario
                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Where(x => x.OwnerId == userId)
                    .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                _vehicles.Clear();
                if (vehiclesResponse?.Models != null)
                {
                    foreach (var vehicle in vehiclesResponse.Models)
                    {
                        _vehicles.Add(vehicle);
                    }
                }

                // Actualizar estadísticas y UI
                await UpdateStatisticsAsync();
                BuildVehiclesUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando vehículos: {ex.Message}");
                await DisplayAlert("Error", "No se pudieron cargar los vehículos", "OK");
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
                ActiveCountLabel.Text = activeCount.ToString();

                // Contar solicitudes pendientes
                var pendingRentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.OwnerId == userId)
                    .Where(x => x.StatusString == "pendiente")
                    .Get();

                var pendingCount = pendingRentalsResponse?.Models?.Count ?? 0;
                RequestsCountLabel.Text = pendingCount.ToString();

                // Calcular ganancias totales
                var completedRentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.OwnerId == userId)
                    .Where(x => x.StatusString == "completada")
                    .Get();

                var totalEarnings = completedRentalsResponse?.Models?.Sum(r => r.TotalPrice) ?? 0;
                EarningsLabel.Text = totalEarnings >= 1000 
                    ? $"${totalEarnings / 1000:F1}K" 
                    : $"${totalEarnings:F0}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando estadísticas: {ex.Message}");
            }
        }

        private void BuildVehiclesUI()
        {
            VehiclesContainer.Children.Clear();

            if (_vehicles.Count == 0)
            {
                // Mostrar estado vacío
                VehiclesContainer.Children.Add(new VerticalStackLayout
                {
                    Spacing = 15,
                    Padding = new Thickness(20),
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "🚗",
                            FontSize = 60,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            Text = "No tienes vehículos registrados",
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.Center,
                            TextColor = GetDynamicColor("TextPrimary")
                        },
                        new Label
                        {
                            Text = "Agrega tu primer vehículo para empezar a rentar",
                            FontSize = 14,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            TextColor = GetDynamicColor("TextSecondary")
                        }
                    }
                });
                return;
            }

            // Agregar título
            VehiclesContainer.Children.Add(new Label
            {
                Text = "Tus Vehículos",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = GetDynamicColor("TextPrimary"),
                Margin = new Thickness(0, 10, 0, 5)
            });

            // Agregar cada vehículo
            foreach (var vehicle in _vehicles)
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
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                RowSpacing = 12
            };

            // Fila 0: Información del vehículo
            var vehicleStack = new HorizontalStackLayout { Spacing = 15 };

            var iconFrame = new Frame
            {
                WidthRequest = 80,
                HeightRequest = 80,
                CornerRadius = 10,
                Padding = 0,
                BackgroundColor = GetVehicleBackgroundColor(vehicle.Brand),
                HasShadow = false,
                Content = new Label
                {
                    Text = GetVehicleEmoji(vehicle.Brand),
                    FontSize = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var infoStack = new VerticalStackLayout
            {
                Spacing = 6,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            infoStack.Children.Add(new Label
            {
                Text = vehicle.DisplayName,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = GetDynamicColor("TextPrimary")
            });

            infoStack.Children.Add(new Label
            {
                Text = $"{vehicle.Seats} asientos • {vehicle.TransmissionDisplay}",
                FontSize = 13,
                TextColor = GetDynamicColor("TextSecondary")
            });

            var statusFrame = new Frame
            {
                Padding = new Thickness(8, 4),
                CornerRadius = 8,
                BackgroundColor = vehicle.IsAvailable ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#FFF3E0"),
                HasShadow = false,
                HorizontalOptions = LayoutOptions.Start,
                Content = new Label
                {
                    Text = vehicle.IsAvailable ? "Disponible" : "Rentado",
                    FontSize = 11,
                    TextColor = vehicle.IsAvailable ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF9800"),
                    FontAttributes = FontAttributes.Bold
                }
            };

            infoStack.Children.Add(statusFrame);

            vehicleStack.Children.Add(iconFrame);
            vehicleStack.Children.Add(infoStack);

            grid.Children.Add(vehicleStack);
            Grid.SetRow(vehicleStack, 0);

            // Fila 1: Separador
            var separator = new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = GetDynamicColor("Divider")
            };
            grid.Children.Add(separator);
            Grid.SetRow(separator, 1);

            // Fila 2: Detalles y botón
            var detailsGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var priceStack = new VerticalStackLayout { Spacing = 4 };
            priceStack.Children.Add(new Label
            {
                Text = "Precio/día",
                FontSize = 11,
                TextColor = GetDynamicColor("TextSecondary")
            });
            priceStack.Children.Add(new Label
            {
                Text = $"${vehicle.PricePerDay:F0}",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = GetStaticColor("Primary")
            });

            var locationStack = new VerticalStackLayout { Spacing = 4 };
            locationStack.Children.Add(new Label
            {
                Text = "Ubicación",
                FontSize = 11,
                TextColor = GetDynamicColor("TextSecondary")
            });
            locationStack.Children.Add(new Label
            {
                Text = vehicle.Location ?? "No especificada",
                FontSize = 12,
                TextColor = GetDynamicColor("TextPrimary"),
                LineBreakMode = LineBreakMode.TailTruncation
            });

            var editButton = new Button
            {
                Text = "Editar",
                BackgroundColor = Colors.Transparent,
                TextColor = GetStaticColor("Primary"),
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center
            };

            detailsGrid.Children.Add(priceStack);
            Grid.SetColumn(priceStack, 0);
            detailsGrid.Children.Add(locationStack);
            Grid.SetColumn(locationStack, 1);
            detailsGrid.Children.Add(editButton);
            Grid.SetColumn(editButton, 2);

            grid.Children.Add(detailsGrid);
            Grid.SetRow(detailsGrid, 2);

            return new Frame
            {
                CornerRadius = 15,
                HasShadow = true,
                BackgroundColor = GetDynamicColor("CardBackground"),
                Padding = new Thickness(15),
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

        private Color GetVehicleBackgroundColor(string brand)
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

        private Color GetDynamicColor(string key)
        {
            if (Application.Current?.Resources.TryGetValue(key, out var color) == true && color is Color c)
            {
                return c;
            }
            return Colors.Black; // Fallback
        }

        private Color GetStaticColor(string key)
        {
            if (Application.Current?.Resources.TryGetValue(key, out var color) == true && color is Color c)
            {
                return c;
            }
            return Colors.Black; // Fallback
        }

        private async void OnAddVehicleClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddVehiclePage());
        }

        private async void OnEarningsClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new OwnerEarningsPage());
        }

        private async void OnRentalsClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new OwnerRentalsPage());
        }
    }
}
