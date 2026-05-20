using AUTORENT.Services;
using AUTORENT.Models;
using System.Collections.ObjectModel;

namespace AUTORENT
{
    public partial class RentalsPage : ContentPage
    {
        private readonly AuthService _authService;
        private ObservableCollection<RentalWithVehicle> _activeRentals = new();
        private ObservableCollection<RentalWithVehicle> _upcomingRentals = new();
        private ObservableCollection<RentalWithVehicle> _completedRentals = new();

        public RentalsPage()
        {
            InitializeComponent();
            _authService = AuthService.Instance;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadRentalsAsync();
        }

        private async Task LoadRentalsAsync()
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

                // Cargar todas las rentas del usuario (como conductor/renter)
                var rentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.RenterId == userId)
                    .Order(x => x.StartDate, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                if (rentalsResponse?.Models == null || rentalsResponse.Models.Count == 0)
                {
                    // No hay rentas, mostrar mensaje
                    ShowEmptyState();
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
                _activeRentals.Clear();
                _upcomingRentals.Clear();
                _completedRentals.Clear();

                foreach (var rv in rentalsWithVehicles)
                {
                    if (rv.Rental.Status == RentalStatus.Completed || rv.Rental.Status == RentalStatus.Cancelled)
                    {
                        _completedRentals.Add(rv);
                    }
                    else if (rv.Rental.StartDate <= now && rv.Rental.EndDate >= now)
                    {
                        _activeRentals.Add(rv);
                    }
                    else if (rv.Rental.StartDate > now)
                    {
                        _upcomingRentals.Add(rv);
                    }
                }

                BuildUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando rentas: {ex.Message}");
                await DisplayAlert("Error", "No se pudieron cargar las rentas", "OK");
            }
        }

        private void ShowEmptyState()
        {
            ContentArea.Children.Clear();
            ContentArea.Children.Add(new VerticalStackLayout
            {
                Spacing = 15,
                Padding = new Thickness(20),
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = "📋",
                        FontSize = 60,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "No tienes rentas aún",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = GetDynamicColor("TextPrimary")
                    },
                    new Label
                    {
                        Text = "Explora vehículos disponibles y haz tu primera reserva",
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = GetDynamicColor("TextSecondary")
                    }
                }
            });
        }

        private void BuildUI()
        {
            ContentArea.Children.Clear();

            // Activas
            if (_activeRentals.Count > 0)
            {
                ContentArea.Children.Add(new Label
                {
                    Text = "Activas",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = GetDynamicColor("TextPrimary"),
                    Margin = new Thickness(0, 10, 0, 5)
                });

                foreach (var rv in _activeRentals)
                {
                    ContentArea.Children.Add(CreateRentalCard(rv, true));
                }
            }

            // Próximas
            if (_upcomingRentals.Count > 0)
            {
                ContentArea.Children.Add(new Label
                {
                    Text = "Próximas",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = GetDynamicColor("TextPrimary"),
                    Margin = new Thickness(0, 15, 0, 5)
                });

                foreach (var rv in _upcomingRentals)
                {
                    ContentArea.Children.Add(CreateRentalCard(rv, false));
                }
            }

            // Historial
            if (_completedRentals.Count > 0)
            {
                ContentArea.Children.Add(new Label
                {
                    Text = "Historial",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = GetDynamicColor("TextPrimary"),
                    Margin = new Thickness(0, 15, 0, 5)
                });

                foreach (var rv in _completedRentals)
                {
                    ContentArea.Children.Add(CreateCompletedRentalCard(rv));
                }
            }

            if (_activeRentals.Count == 0 && _upcomingRentals.Count == 0 && _completedRentals.Count == 0)
            {
                ShowEmptyState();
            }
        }

        private Frame CreateRentalCard(RentalWithVehicle rv, bool isActive)
        {
            var vehicle = rv.Vehicle!;
            var rental = rv.Rental;

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                Padding = new Thickness(20),
                RowSpacing = 12
            };

            // Fila 0: Vehículo
            var vehicleStack = new HorizontalStackLayout
            {
                Spacing = 15
            };

            vehicleStack.Children.Add(new Label
            {
                Text = GetVehicleEmoji(vehicle.Brand),
                FontSize = 40,
                VerticalOptions = LayoutOptions.Center
            });

            var infoStack = new VerticalStackLayout
            {
                Spacing = 4,
                VerticalOptions = LayoutOptions.Center
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

            // Fila 2: Fechas
            var datesGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = 10
            };

            var pickupStack = new VerticalStackLayout { Spacing = 4 };
            pickupStack.Children.Add(new Label
            {
                Text = "Recogida",
                FontSize = 12,
                TextColor = GetDynamicColor("TextSecondary")
            });
            pickupStack.Children.Add(new Label
            {
                Text = rental.StartDate.ToString("dd MMM yyyy"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = GetDynamicColor("TextPrimary")
            });
            if (isActive)
            {
                pickupStack.Children.Add(new Label
                {
                    Text = rental.StartDate.ToString("HH:mm"),
                    FontSize = 12,
                    TextColor = GetStaticColor("Primary")
                });
            }

            var returnStack = new VerticalStackLayout { Spacing = 4 };
            returnStack.Children.Add(new Label
            {
                Text = "Devolución",
                FontSize = 12,
                TextColor = GetDynamicColor("TextSecondary")
            });
            returnStack.Children.Add(new Label
            {
                Text = rental.EndDate.ToString("dd MMM yyyy"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = GetDynamicColor("TextPrimary")
            });
            if (isActive)
            {
                returnStack.Children.Add(new Label
                {
                    Text = rental.EndDate.ToString("HH:mm"),
                    FontSize = 12,
                    TextColor = GetStaticColor("Primary")
                });
            }

            datesGrid.Children.Add(pickupStack);
            Grid.SetColumn(pickupStack, 0);
            datesGrid.Children.Add(returnStack);
            Grid.SetColumn(returnStack, 1);

            grid.Children.Add(datesGrid);
            Grid.SetRow(datesGrid, 2);

            return new Frame
            {
                CornerRadius = 15,
                HasShadow = true,
                BackgroundColor = GetDynamicColor("CardBackground"),
                Padding = 0,
                Content = grid
            };
        }

        private Frame CreateCompletedRentalCard(RentalWithVehicle rv)
        {
            var vehicle = rv.Vehicle!;
            var rental = rv.Rental;

            var stack = new HorizontalStackLayout
            {
                Spacing = 15,
                Padding = new Thickness(20)
            };

            stack.Children.Add(new Label
            {
                Text = GetVehicleEmoji(vehicle.Brand),
                FontSize = 40,
                VerticalOptions = LayoutOptions.Center
            });

            var infoStack = new VerticalStackLayout
            {
                Spacing = 4,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            infoStack.Children.Add(new Label
            {
                Text = vehicle.DisplayName,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = GetDynamicColor("TextPrimary")
            });

            var statusText = rental.Status == RentalStatus.Completed ? "Completada" : "Cancelada";
            var dateRange = $"{rental.StartDate:dd MMM} - {rental.EndDate:dd MMM yyyy}";

            infoStack.Children.Add(new Label
            {
                Text = $"{statusText} • {dateRange}",
                FontSize = 12,
                TextColor = GetDynamicColor("TextSecondary")
            });

            stack.Children.Add(infoStack);

            stack.Children.Add(new Label
            {
                Text = rental.Status == RentalStatus.Completed ? "✓" : "✕",
                FontSize = 24,
                TextColor = rental.Status == RentalStatus.Completed ? Color.FromArgb("#4CAF50") : Color.FromArgb("#F44336"),
                VerticalOptions = LayoutOptions.Center
            });

            return new Frame
            {
                CornerRadius = 15,
                HasShadow = true,
                BackgroundColor = GetDynamicColor("CardBackground"),
                Padding = 0,
                Content = stack
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
    }

    public class RentalWithVehicle
    {
        public Rental Rental { get; set; } = null!;
        public Vehicle? Vehicle { get; set; }
    }
}
