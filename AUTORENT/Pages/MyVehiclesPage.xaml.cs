using AUTORENT.ViewModels;

namespace AUTORENT.Pages
{
    public partial class MyVehiclesPage : ContentPage
    {
        private readonly MyVehiclesViewModel _viewModel;

        public MyVehiclesPage()
        {
            InitializeComponent();
            _viewModel = new MyVehiclesViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadVehiclesAsync();
            BuildVehiclesUI();
        }

        private void BuildVehiclesUI()
        {
            VehiclesContainer.Children.Clear();

            if (_viewModel.Vehicles.Count == 0)
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
            foreach (var vehicle in _viewModel.Vehicles)
            {
                VehiclesContainer.Children.Add(CreateVehicleCard(vehicle));
            }
        }

        private Frame CreateVehicleCard(Models.Vehicle vehicle)
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
                BackgroundColor = _viewModel.GetVehicleBackgroundColor(vehicle.Brand),
                HasShadow = false,
                Content = new Label
                {
                    Text = _viewModel.GetVehicleEmoji(vehicle.Brand),
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
