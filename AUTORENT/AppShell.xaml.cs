using AUTORENT.Services;
using AUTORENT.Pages;

namespace AUTORENT
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Aplicar tema
            Shell.SetTabBarBackgroundColor(this, Colors.White);
            Shell.SetTabBarForegroundColor(this, Color.FromArgb("#1E88E5"));
            Shell.SetTabBarUnselectedColor(this, Color.FromArgb("#9E9E9E"));
            Shell.SetTabBarTitleColor(this, Color.FromArgb("#212121"));

            // Registrar rutas
            Routing.RegisterRoute(nameof(AddVehiclePage), typeof(AddVehiclePage));
            Routing.RegisterRoute(nameof(EditVehiclePage), typeof(EditVehiclePage));
            Routing.RegisterRoute(nameof(OwnerEarningsPage), typeof(OwnerEarningsPage));
            Routing.RegisterRoute(nameof(OwnerRentalsPage), typeof(OwnerRentalsPage));
            Routing.RegisterRoute(nameof(VehicleDetailPage), typeof(VehicleDetailPage));

            // Mostrar tabs según el rol
            ConfigureTabsForUserRole();
        }

        private void ConfigureTabsForUserRole()
        {
            var authService = AuthService.Instance;

            if (authService.IsOwner())
            {
                // Propietario: Mis Autos, Solicitudes, Perfil
                DriverTabs.IsVisible = false;
                OwnerTabs.IsVisible = true;
                CurrentItem = OwnerTabs;
            }
            else
            {
                // Conductor: Inicio (catálogo), Mis Rentas, Perfil
                DriverTabs.IsVisible = true;
                OwnerTabs.IsVisible = false;
                CurrentItem = DriverTabs;
            }
        }
    }
}
