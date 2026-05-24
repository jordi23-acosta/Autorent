namespace AUTORENT.Pages
{
    public partial class OwnerEarningsPage : ContentPage
    {
        public OwnerEarningsPage()
        {
            InitializeComponent();
        }

        private async void OnWithdrawClicked(object? sender, EventArgs e)
        {
            await DisplayAlert(
                "💸 Retiro de Fondos",
                "Tu solicitud de retiro será procesada en 2-3 días hábiles.\n\nMonto disponible: $12,450",
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
