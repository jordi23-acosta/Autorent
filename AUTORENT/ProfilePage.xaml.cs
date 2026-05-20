using AUTORENT.ViewModels;
using AUTORENT.Services;

namespace AUTORENT
{
    public partial class ProfilePage : ContentPage
    {
        private ProfileViewModel ViewModel => (ProfileViewModel)BindingContext;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
            
            // Suscribirse a cambios de tema
            ThemeService.Instance.ThemeChanged += (s, theme) => ApplyTheme();
            
            ApplyTheme();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadUserData();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;

            // Colores de fondo
            Color pageBackground = isDarkMode ? Color.FromArgb("#121212") : Color.FromArgb("#F5F5F5");
            Color cardBackground = isDarkMode ? Color.FromArgb("#1E1E1E") : Colors.White;
            Color textPrimary = isDarkMode ? Colors.White : Color.FromArgb("#212121");
            Color textSecondary = isDarkMode ? Color.FromArgb("#B0B0B0") : Color.FromArgb("#757575");

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
    }
}
