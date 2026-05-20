using AUTORENT.Services;

namespace AUTORENT
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = AuthService.Instance;
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || 
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                return;
            }

            LoginButton.IsEnabled = false;
            LoginButton.Text = "Iniciando sesión...";

            var (success, message, user) = await _authService.LoginAsync(
                EmailEntry.Text.Trim(), 
                PasswordEntry.Text);

            LoginButton.IsEnabled = true;
            LoginButton.Text = "Iniciar Sesión";

            if (success && user != null)
            {
                await DisplayAlert("Bienvenido", $"Hola {user.Name}!", "Continuar");
                
                // Navegar al AppShell principal
                Application.Current!.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Error", message, "OK");
            }
        }

        private async void OnRegisterClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private async void OnForgotPasswordTapped(object? sender, EventArgs e)
        {
            await DisplayAlert("Recuperar Contraseña", 
                "Se enviará un enlace de recuperación a tu email", "OK");
        }

        private void OnTogglePasswordClicked(object? sender, EventArgs e)
        {
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
            TogglePasswordButton.Text = PasswordEntry.IsPassword ? "👁" : "🙈";
        }
    }
}
