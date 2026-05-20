using AutoRent.Models;
using AutoRent.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace AutoRent.ViewModels;

public class ConfirmRentViewModel : BaseViewModel
{
    private readonly SupabaseService _supabase;

    public string AutoInfo { get; set; }
    public string DurationLabel { get; set; }
    public decimal Total { get; set; }
    public string TotalFormatted => Total.ToString("C");

    public ICommand ConfirmCommand { get; }

    public ConfirmRentViewModel()
    {
        _supabase = App.Services.GetService(typeof(SupabaseService)) as SupabaseService;
        ConfirmCommand = new Command(async () => await ConfirmAsync());
    }

    [QueryProperty("rentaTipo", "rentaTipo")]
    public void LoadFromQueryParameters(string rentaTipo, string autoId, string duracion, string total)
    {
        // Este método puede ser llamado desde code-behind si es necesario
    }

    public async Task InitializeFromQueryAsync(string rentaTipo, int autoId, int duracion, decimal total)
    {
        // Para simplificar, cargamos el auto brevemente
        var autos = await _supabase.GetAutosAsync();
        var auto = autos.FirstOrDefault(a => a.Id == autoId);
        AutoInfo = auto != null ? $"{auto.Marca} {auto.Modelo} ({auto.Ano})" : $"Auto #{autoId}";
        DurationLabel = rentaTipo == "hora" ? $"{duracion} horas" : $"{duracion} días";
        Total = total;
        OnPropertyChanged(nameof(AutoInfo));
        OnPropertyChanged(nameof(DurationLabel));
        OnPropertyChanged(nameof(TotalFormatted));
    }

    private async Task ConfirmAsync()
    {
        // Crear la renta y guardarla
        var userJson = await SecureStorage.GetAsync("supabase_user");
        var userId = "";
        if (!string.IsNullOrWhiteSpace(userJson))
        {
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(userJson);
                if (doc.RootElement.TryGetProperty("id", out var id)) userId = id.GetString();
            }
            catch { }
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Usuario no autenticado", "OK");
            return;
        }

        // Aquí tendríamos los detalles, pero para ejemplo usamos valores hardcodeados
        var renta = new Renta
        {
            UsuarioId = userId,
            AutoId = 0,
            FechaInicio = DateTime.UtcNow,
            Duracion = 1,
            TipoRenta = "hora",
            Total = Total,
            Estado = "activa"
        };

        var resp = await _supabase.CreateRentaAsync(renta);
        if (resp.IsSuccessStatusCode)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Renta registrada", "OK");
            await Shell.Current.GoToAsync("/cars");
        }
        else
        {
            var msg = await resp.Content.ReadAsStringAsync();
            await Application.Current.MainPage.DisplayAlert("Error", msg, "OK");
        }
    }
}
