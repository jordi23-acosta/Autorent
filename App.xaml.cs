using Microsoft.Maui.Controls;

namespace AutoRent;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public App(IServiceProvider services)
    {
        InitializeComponent();
        Services = services;

        // Usaremos Shell para navegación más fácil
        MainPage = services.GetService<AppShell>() ?? new AppShell();
    }
}
