namespace AUTORENT.Pages
{
    public partial class OwnerRentalsPage : ContentPage
    {
        public OwnerRentalsPage()
        {
            InitializeComponent();
        }

        private async void OnAcceptClicked(object? sender, EventArgs e)
        {
            bool answer = await DisplayAlert(
                "✓ Aceptar Solicitud",
                "¿Confirmas que aceptas esta renta?\n\nEl cliente recibirá una notificación.",
                "Sí, aceptar",
                "Cancelar");

            if (answer)
            {
                await DisplayAlert(
                    "🎉 ¡Confirmado!",
                    "La solicitud ha sido aceptada exitosamente",
                    "OK");
            }
        }

        private async void OnRejectClicked(object? sender, EventArgs e)
        {
            bool answer = await DisplayAlert(
                "✕ Rechazar Solicitud",
                "¿Estás seguro que deseas rechazar esta renta?",
                "Sí, rechazar",
                "Cancelar");

            if (answer)
            {
                await DisplayAlert(
                    "Rechazada",
                    "La solicitud ha sido rechazada",
                    "OK");
            }
        }

        private async void OnViewDetailsClicked(object? sender, EventArgs e)
        {
            await DisplayAlert(
                "📋 Detalles de Renta",
                "Aquí verás información detallada de la renta activa",
                "OK");
        }

        private async void OnBackTapped(object? sender, TappedEventArgs e)
        {
            try
            {
                if (Navigation?.NavigationStack?.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
            catch { }
        }
    }
}
