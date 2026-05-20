using AUTORENT.ViewModels;

namespace AUTORENT.Pages
{
    public partial class AddVehiclePage : ContentPage
    {
        private readonly AddVehicleViewModel _viewModel;

        public AddVehiclePage()
        {
            InitializeComponent();
            _viewModel = new AddVehicleViewModel();
            BindingContext = _viewModel;
        }

        // ─── FOTOS (Requiere acceso a UI, no puede estar en ViewModel) ───────

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

            // Actualizar ViewModel
            _viewModel.SetPhotoPath(slot, photo.FullPath);

            // Actualizar UI
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
        }

        // ─── CATEGORÍA (Requiere acceso a UI, no puede estar en ViewModel) ───

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
                    break;
                case "SUV":
                    SetCategoryBorder(SUVBorder);
                    break;
                case "Luxury":
                    SetCategoryBorder(LuxuryBorder);
                    break;
                case "Van":
                    SetCategoryBorder(VanBorder);
                    break;
            }

            // Actualizar ViewModel
            _viewModel.SelectCategory(category);
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
    }
}
