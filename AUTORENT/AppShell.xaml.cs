using AUTORENT.Services;
using AUTORENT.Pages;

namespace AUTORENT
{
    public partial class AppShell : Shell
    {
        private readonly ThemeService _themeService;

        public AppShell()
        {
            InitializeComponent();

            // Inicializar servicio de temas
            _themeService = ThemeService.Instance;
            _themeService.ThemeChanged += OnThemeChanged;

            // Aplicar tema inicial
            ApplyTheme(_themeService.IsDarkMode());

            // Registrar rutas
            Routing.RegisterRoute(nameof(MyVehiclesPage), typeof(MyVehiclesPage));
            Routing.RegisterRoute(nameof(AddVehiclePage), typeof(AddVehiclePage));
            Routing.RegisterRoute(nameof(OwnerEarningsPage), typeof(OwnerEarningsPage));
            Routing.RegisterRoute(nameof(OwnerRentalsPage), typeof(OwnerRentalsPage));

            // Configurar tabs según el rol del usuario
            ConfigureTabsForUserRole();
        }

        private void OnThemeChanged(object? sender, AppTheme theme)
        {
            ApplyTheme(theme == AppTheme.Dark);
        }

        private void ApplyTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                Shell.SetTabBarBackgroundColor(this, Color.FromArgb("#1E1E1E"));
                Shell.SetTabBarForegroundColor(this, Color.FromArgb("#1E88E5"));
                Shell.SetTabBarUnselectedColor(this, Color.FromArgb("#757575"));
                Shell.SetTabBarTitleColor(this, Color.FromArgb("#FFFFFF"));
            }
            else
            {
                Shell.SetTabBarBackgroundColor(this, Colors.White);
                Shell.SetTabBarForegroundColor(this, Color.FromArgb("#1E88E5"));
                Shell.SetTabBarUnselectedColor(this, Color.FromArgb("#9E9E9E"));
                Shell.SetTabBarTitleColor(this, Color.FromArgb("#212121"));
            }
        }

        private void ConfigureTabsForUserRole()
        {
            var authService = AuthService.Instance;
            
            if (authService.IsOwner())
            {
                // Para propietarios, mostrar "Mis Vehículos" en lugar de "Favoritos"
                // Esto se puede hacer dinámicamente o con binding
                Title = "AUTORENT - Propietario";
            }
            else
            {
                Title = "AUTORENT - Conductor";
            }
        }
    }
}
