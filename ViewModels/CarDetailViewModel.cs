using AutoRent.Models;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace AutoRent.ViewModels;

public class CarDetailViewModel : BaseViewModel
{
    public Auto Auto { get; set; }

    public List<string> RentTypes { get; } = new() { "hora", "dia" };

    private string _selectedRentType = "hora";
    public string SelectedRentType { get => _selectedRentType; set { SetProperty(ref _selectedRentType, value); Recalculate(); } }

    private int _duration = 1;
    public int Duration { get => _duration; set { SetProperty(ref _duration, value); Recalculate(); } }

    public string DurationLabel => SelectedRentType == "hora" ? $"{Duration} horas" : $"{Duration} días";

    private decimal _total;
    public decimal Total { get => _total; set => SetProperty(ref _total, value); }

    public string TotalFormatted => Total.ToString("C");

    public ICommand ConfirmCommand { get; }

    public CarDetailViewModel()
    {
        // Auto de ejemplo (en tiempo real se inyectaría o recibiría por parámetro)
        Auto = new Auto { Marca = "Marca", Modelo = "Modelo", Ano = 2023, PrecioHora = 10, PrecioDia = 80, ImagenUrl = "https://via.placeholder.com/400x200" };
        ConfirmCommand = new Command(async () => await Shell.Current.GoToAsync($"/confirm?rentaTipo={SelectedRentType}&autoId={Auto.Id}&duracion={Duration}&total={Total}"));
        Recalculate();
    }

    private void Recalculate()
    {
        if (Auto == null) return;
        if (SelectedRentType == "hora")
            Total = Auto.PrecioHora * Duration;
        else
            Total = Auto.PrecioDia * Duration;
        OnPropertyChanged(nameof(TotalFormatted));
        OnPropertyChanged(nameof(DurationLabel));
    }
}
