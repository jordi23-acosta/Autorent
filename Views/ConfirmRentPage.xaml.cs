using Microsoft.Maui.Controls;

namespace AutoRent.Views;

[QueryProperty(nameof(RentaTipo), "rentaTipo")]
[QueryProperty(nameof(AutoId), "autoId")]
[QueryProperty(nameof(Duracion), "duracion")]
[QueryProperty(nameof(Total), "total")]
public partial class ConfirmRentPage : ContentPage
{
    public string RentaTipo { get; set; }
    public string AutoId { get; set; }
    public string Duracion { get; set; }
    public string Total { get; set; }

    public ConfirmRentPage()
    {
        InitializeComponent();
    }
}
