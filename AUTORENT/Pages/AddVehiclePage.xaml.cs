using AUTORENT.Models;
using AUTORENT.Services;

namespace AUTORENT.Pages
{
    public partial class AddVehiclePage : ContentPage
    {
        private int _seats = 5;
        private VehicleCategory _selectedCategory = VehicleCategory.Economic;
        private readonly string?[] _photoPaths = new string?[3];
        private int _photoCount = 0;
        private readonly AuthService _authService;

        public AddVehiclePage()
        {
            InitializeComponent();
            _authService = AuthService.Instance;
        }

        // ─── FOTOS ───────────────────────────────────────────────────────────

        private async void OnAddPhotoTapped(object? sender, TappedEventArgs e)
        {
            int slot = e.Parameter is string s && int.TryParse(s, out int idx) ? idx : 0;

            string action = await DisplayActionSheet(
                "Agregar foto", "Cancelar", null,
                "📷 Tomar foto", "🖼️ Elegir de galería");

            FileResult? photo = null;

            try
            {
                if (action == "📷 Tomar foto")
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                        photo = await MediaPicker.Default.CapturePhotoAsync();
                }
                else if (action == "🖼️ Elegir de galería")
                {
                    photo = await MediaPicker.Default.PickPhotoAsync();
                }
            }
            catch
            {
                await DisplayAlert("Error", "No se pudo acceder a la cámara o galería", "OK");
                return;
            }

            if (photo == null) return;

            // Mostrar la foto en el slot correspondiente
            var stream = await photo.OpenReadAsync();
            var imageSource = ImageSource.FromStream(() => stream);

            bool wasEmpty = _photoPaths[slot] == null;
            _photoPaths[slot] = photo.FullPath;

            switch (slot)
            {
                case 0:
                    Photo1.Source = imageSource;
                    Photo1.IsVisible = true;
                    AddPhoto1.IsVisible = false;
                    break;
                case 1:
                    Photo2.Source = imageSource;
                    Photo2.IsVisible = true;
                    AddPhoto2.IsVisible = false;
                    break;
                case 2:
                    Photo3.Source = imageSource;
                    Photo3.IsVisible = true;
                    AddPhoto3.IsVisible = false;
                    break;
            }

            if (wasEmpty) _photoCount++;
            UpdatePhotoCount();
        }

        private void UpdatePhotoCount()
        {
            PhotoCountLabel.Text = $"{_photoCount} de 3 fotos mínimas";
            PhotoCountLabel.TextColor = _photoCount >= 3
                ? Color.FromArgb("#4CAF50")
                : Color.FromArgb("#FF5722");
        }

        // ─── CATEGORÍA ───────────────────────────────────────────────────────

        private void OnCategoryTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is not string category) return;

            // Reset todos
            ResetCategoryBorder(EconomicBorder);
            ResetCategoryBorder(SUVBorder);
            ResetCategoryBorder(LuxuryBorder);
            ResetCategoryBorder(VanBorder);

            // Activar el seleccionado
            switch (category)
            {
                case "Economic":
                    SetCategoryBorder(EconomicBorder);
                    _selectedCategory = VehicleCategory.Economic;
                    break;
                case "SUV":
                    SetCategoryBorder(SUVBorder);
                    _selectedCategory = VehicleCategory.SUV;
                    break;
                case "Luxury":
                    SetCategoryBorder(LuxuryBorder);
                    _selectedCategory = VehicleCategory.Luxury;
                    break;
                case "Van":
                    SetCategoryBorder(VanBorder);
                    _selectedCategory = VehicleCategory.Van;
                    break;
            }
        }

        private static void ResetCategoryBorder(Border b)
        {
            b.Stroke = Color.FromArgb("#E0E0E0");
            b.StrokeThickness = 1.5;
            b.BackgroundColor = Colors.White;
        }

        private static void SetCategoryBorder(Border b)
        {
            b.Stroke = Color.FromArgb("#1E88E5");
            b.StrokeThickness = 2;
            b.BackgroundColor = Color.FromArgb("#E3F2FD");
        }

        // ─── ASIENTOS ────────────────────────────────────────────────────────

        private void OnSeatsDecrement(object? sender, EventArgs e)
        {
            if (_seats > 2) { _seats--; SeatsLabel.Text = _seats.ToString(); }
        }

        private void OnSeatsIncrement(object? sender, EventArgs e)
        {
            if (_seats < 12) { _seats++; SeatsLabel.Text = _seats.ToString(); }
        }

        // ─── PUBLICAR ────────────────────────────────────────────────────────

        private async void OnPublishClicked(object? sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(BrandEntry.Text) ||
                string.IsNullOrWhiteSpace(ModelEntry.Text) ||
                string.IsNullOrWhiteSpace(YearEntry.Text) ||
                string.IsNullOrWhiteSpace(PlatesEntry.Text) ||
                string.IsNullOrWhiteSpace(ColorEntry.Text) ||
                string.IsNullOrWhiteSpace(PriceEntry.Text))
            {
                await DisplayAlert("Campos incompletos", "Por favor completa todos los campos obligatorios (*)", "OK");
                return;
            }

            if (_photoCount < 1)
            {
                await DisplayAlert("Fotos requeridas", "Agrega al menos 1 foto de tu vehículo", "OK");
                return;
            }

            if (!int.TryParse(YearEntry.Text, out int year) || year < 1990 || year > DateTime.Now.Year + 1)
            {
                await DisplayAlert("Año inválido", "Ingresa un año entre 1990 y el año actual", "OK");
                return;
            }

            if (!decimal.TryParse(PriceEntry.Text, out decimal price) || price <= 0)
            {
                await DisplayAlert("Precio inválido", "Ingresa un precio mayor a $0", "OK");
                return;
            }

            try
            {
                var client = _authService.GetClient();
                if (client == null)
                {
                    await DisplayAlert("Error", "No se pudo conectar con el servidor", "OK");
                    return;
                }

                var vehicle = new Vehicle
                {
                    Brand = BrandEntry.Text.Trim(),
                    Model = ModelEntry.Text.Trim(),
                    Year = year,
                    Plate = PlatesEntry.Text.Trim().ToUpper(),
                    Color = ColorEntry.Text.Trim(),
                    Seats = _seats,
                    Transmission = AutomaticSwitch.IsToggled ? "automatico" : "manual",
                    PricePerDay = price,
                    Description = DescriptionEditor.Text?.Trim() ?? "",
                    Location = LocationEntry.Text?.Trim() ?? "",
                    IsAvailable = true,
                    ImageUrl = _photoPaths[0],
                    OwnerId = _authService.CurrentUser!.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] Guardando vehículo: {vehicle.Brand} {vehicle.Model}");
                
                // Guardar en Supabase
                var response = await client.From<Vehicle>().Insert(vehicle);
                
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] ✅ Vehículo guardado exitosamente");

                await DisplayAlert("¡Publicado! 🎉",
                    $"Tu {vehicle.Brand} {vehicle.Model} {vehicle.Year} ya está disponible para rentar.",
                    "Ver mis autos");

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ADD VEHICLE] ❌ Error: {ex.Message}");
                await DisplayAlert("Error", $"No se pudo publicar el vehículo: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object? sender, EventArgs e)
        {
            bool answer = await DisplayAlert("¿Cancelar?",
                "Se perderán los datos ingresados", "Sí, cancelar", "Seguir editando");
            if (answer) await Navigation.PopAsync();
        }
    }
}
