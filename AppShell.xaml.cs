using Microsoft.Maui.Controls;

namespace AutoRent;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Registrar rutas
        Routing.RegisterRoute("login", typeof(Views.LoginPage));
        Routing.RegisterRoute("cars", typeof(Views.CarsPage));
        Routing.RegisterRoute("cardetail", typeof(Views.CarDetailPage));
        Routing.RegisterRoute("confirm", typeof(Views.ConfirmRentPage));
        Routing.RegisterRoute("admin", typeof(Views.AdminPanelPage));
        Routing.RegisterRoute("admin/edit", typeof(Views.AdminCarEditPage));
    }
}
