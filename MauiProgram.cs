using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace AutoRent;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        // Registrar servicios y ViewModels
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<Views.MainPage>();
        builder.Services.AddTransient<ViewModels.MainViewModel>();

        // HttpClient para Supabase
        builder.Services.AddHttpClient("Supabase");

        // Registrar SupabaseService
        builder.Services.AddSingleton<Services.SupabaseService>();

        // Registrar otras vistas y viewmodels (placeholders)
        builder.Services.AddSingleton<Views.CarsPage>();
        builder.Services.AddTransient<ViewModels.CarsViewModel>();
        builder.Services.AddSingleton<Views.LoginPage>();
        builder.Services.AddTransient<ViewModels.LoginViewModel>();
        builder.Services.AddSingleton<Views.CarDetailPage>();
        builder.Services.AddTransient<ViewModels.CarDetailViewModel>();
        builder.Services.AddSingleton<Views.ConfirmRentPage>();
        builder.Services.AddTransient<ViewModels.ConfirmRentViewModel>();
        builder.Services.AddSingleton<Views.AdminPanelPage>();
        builder.Services.AddSingleton<Views.AdminCarEditPage>();
        builder.Services.AddTransient<ViewModels.AdminViewModel>();

        return builder.Build();
    }
}
