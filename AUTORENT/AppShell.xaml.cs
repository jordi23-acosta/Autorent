using AUTORENT.Services;
using AUTORENT.Pages;

namespace AUTORENT
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Aplicar tema claro por defecto (eliminado dark mode)
            ApplyTheme();

            // Registrar rutas
            Routing.RegisterRoute(nameof(MyVehiclesPage), typeof(MyVehiclesPage));
            Routing.RegisterRoute(nameof(AddVehiclePage), typeof(AddVehiclePage));
            Routing.RegisterRoute(nameof(OwnerEarningsPage), typeof(OwnerEarningsPage));
            Routing.RegisterRoute(nameof(OwnerRentalsPage), typeof(OwnerRentalsPage));
            Routing.RegisterRoute(nameof(VehicleDetailPage), typeof(VehicleDetailPage));

            // Configurar tabs según el rol del usuario
            ConfigureTabsForUserRole();
        }

        private void ApplyTheme()
        {
            Shell.SetTabBarBackgroundColor(this, Colors.White);
            Shell.SetTabBarForegroundColor(this, Color.FromArgb("#1E88E5"));
            Shell.SetTabBarUnselectedColor(this, Color.FromArgb("#9E9E9E"));
            Shell.SetTabBarTitleColor(this, Color.FromArgb("#212121"));
        }

        private void ConfigureTabsForUserRole()
        {
            var authService = AuthService.Instance;

            if (authService.IsOwner())
            {
                // Para propietarios: mostrar tab "Mis Autos"
                Title = "AUTORENT - Propietario";
                MyVehiclesTab.IsVisible = true;
            }
            else
            {
                // Para conductores: ocultar tab "Mis Autos"
                Title = "AUTORENT - Conductor";
                MyVehiclesTab.IsVisible = false;
            }
        }
    }
}
