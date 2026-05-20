using System.Windows.Input;
using AutoRent.Services;
using Microsoft.Maui.Controls;

namespace AutoRent.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly SupabaseService _supabase;

    private string _email;
    public string Email { get => _email; set => SetProperty(ref _email, value); }

    private string _password;
    public string Password { get => _password; set => SetProperty(ref _password, value); }

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand RecoverCommand { get; }

    public LoginViewModel()
    {
        _supabase = App.Services.GetService(typeof(SupabaseService)) as SupabaseService;
        LoginCommand = new Command(async () => await LoginAsync());
        RegisterCommand = new Command(async () => await RegisterAsync());
        RecoverCommand = new Command(async () => await RecoverAsync());
    }

    private bool ValidateCredentials()
    {
        return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
    }

    private async Task LoginAsync()
    {
        if (!ValidateCredentials())
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Introduce email y contraseña", "OK");
            return;
        }

        var resp = await _supabase.LoginAsync(Email.Trim(), Password);
        if (resp.IsSuccessStatusCode)
        {
            await Shell.Current.GoToAsync("//cars");
        }
        else
        {
            var msg = await resp.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Login fallido", msg, "OK");
        }
    }

    private async Task RegisterAsync()
    {
        if (!ValidateCredentials())
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Introduce email y contraseña", "OK");
            return;
        }

        var resp = await _supabase.RegisterAsync(Email.Trim(), Password);
        if (resp.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Registro", "Usuario registrado. Revisa tu email para confirmar.", "OK");
        }
        else
        {
            var msg = await resp.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Registro fallido", msg, "OK");
        }
    }

    private async Task RecoverAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Introduce tu email para recuperar la contraseña", "OK");
            return;
        }

        var client = new System.Net.Http.HttpClient();
        var payload = new { email = Email.Trim() };
        var content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
        var url = $"{ApiConstants.AuthUrl}/recover"; // endpoint de Supabase para recover
        var resp = await client.PostAsync(url, content);
        if (resp.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Recuperación", "Email enviado con instrucciones.", "OK");
        }
        else
        {
            var msg = await resp.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Error", msg, "OK");
        }
    }
}
