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
            await DisplayAlert("Retiro de Fondos", 
                "Tu solicitud de retiro será procesada en 2-3 días hábiles", "OK");
        }
    }
}
