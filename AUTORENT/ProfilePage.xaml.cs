using AUTORENT.Services;

namespace AUTORENT
{
    public partial class ProfilePage : ContentPage
    {
        private readonly AuthService _authService;

        public ProfilePage()
        {
            InitializeComponent();
            _authService = AuthService.Instance;
            
            // Suscribirse a cambios de tema
            ThemeService.Instance.ThemeChanged += (s, theme) => ApplyTheme();
            
            LoadUserData();
            LoadDarkModePreference();
            ApplyTheme();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData();
            ApplyTheme();
        }

        private void LoadUserData()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                UserNameLabel.Text = user.Name;
                UserEmailLabel.Text = user.Email;
                UserRoleLabel.Text = user.Role == Models.UserRole.Driver ? "Conductor" : "Propietario";
            }
        }

        private void LoadDarkModePreference()
        {
            // Cargar preferencia guardada
            bool isDarkMode = ThemeService.Instance.IsDarkMode();
            DarkModeSwitch.IsToggled = isDarkMode;
        }

        private void ApplyTheme()
        {
            bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;

            // Colores de fondo
            Color pageBackground = isDarkMode ? Color.FromArgb("#121212") : Color.FromArgb("#F5F5F5");
            Color cardBackground = isDarkMode ? Color.FromArgb("#1E1E1E") : Colors.White;
            Color textPrimary = isDarkMode ? Colors.White : Color.FromArgb("#212121");
            Color textSecondary = isDarkMode ? Color.FromArgb("#B0B0B0") : Color.FromArgb("#757575");
            Color dividerColor = isDarkMode ? Color.FromArgb("#2C2C2C") : Color.FromArgb("#F5F5F5");

            // Aplicar fondo de página
            MainContainer.BackgroundColor = pageBackground;

            // Aplicar a frames de estadísticas
            StatsFrame1.BackgroundColor = cardBackground;
            StatsFrame2.BackgroundColor = cardBackground;
            StatsFrame3.BackgroundColor = cardBackground;
            RentasLabel.TextColor = textSecondary;
            FavoritosLabel.TextColor = textSecondary;
            PremiumLabel.TextColor = textSecondary;

            // Aplicar a secciones
            CuentaLabel.TextColor = textPrimary;
            PreferenciasLabel.TextColor = textPrimary;

            // Aplicar a frames
            CuentaFrame.BackgroundColor = cardBackground;
            PreferenciasFrame.BackgroundColor = cardBackground;

            // Aplicar a labels
            PersonalInfoLabel.TextColor = textPrimary;
            ModoOscuroLabel.TextColor = textPrimary;
        }

        private async void OnPersonalInfoClicked(object? sender, EventArgs e)
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            // Mostrar información personal editable
            string name = await DisplayPromptAsync("Nombre Completo", 
                "Ingresa tu nombre completo:", 
                initialValue: user.Name, 
                maxLength: 100,
                keyboard: Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(name) && name != user.Name)
            {
                try
                {
                    var client = _authService.GetClient();
                    if (client != null)
                    {
                        // Actualizar en Supabase
                        await client
                            .From<Models.Profile>()
                            .Where(x => x.Id == user.Id)
                            .Set(x => x.FullName, name)
                            .Set(x => x.UpdatedAt, DateTime.UtcNow)
                            .Update();

                        // Actualizar localmente
                        user.Name = name;
                        UserNameLabel.Text = name;

                        await DisplayAlert("✅ Éxito", "Tu información ha sido actualizada", "OK");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error actualizando perfil: {ex.Message}");
                    await DisplayAlert("❌ Error", "No se pudo actualizar la información", "OK");
                }
            }
        }

        private void OnDarkModeToggled(object? sender, ToggledEventArgs e)
        {
            bool isDarkMode = e.Value;
            
            // Usar el servicio de temas para cambiar globalmente
            ThemeService.Instance.SetTheme(isDarkMode);

            // Mostrar mensaje
            string message = isDarkMode ? "Modo oscuro activado" : "Modo claro activado";
            DisplayAlert("Tema", message, "OK");
        }

        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Cerrar Sesión", 
                "¿Estás seguro que deseas cerrar sesión?", "Sí", "No");
            
            if (answer)
            {
                _authService.Logout();
                Application.Current!.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
