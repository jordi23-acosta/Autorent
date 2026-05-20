using AUTORENT.Services;

namespace AUTORENT
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authService;

        public RegisterPage()
        {
            InitializeComponent();
            _authService = AuthService.Instance;
        }

        private async void OnRegisterClicked(object? sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PhoneEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                return;
            }

            if (PasswordEntry.Text.Length < 6)
            {
                await DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres", "OK");
                return;
            }

            if (UserTypePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Por favor selecciona tu tipo de usuario", "OK");
                return;
            }

            RegisterButton.IsEnabled = false;
            RegisterButton.Text = "Creando cuenta...";

            // Determinar tipo de usuario
            string userType = UserTypePicker.SelectedIndex == 0 ? "conductor" : "propietario";

            var (success, message, user) = await _authService.RegisterAsync(
                EmailEntry.Text.Trim(),
                PasswordEntry.Text,
                NameEntry.Text.Trim(),
                PhoneEntry.Text.Trim(),
                userType
            );

            RegisterButton.IsEnabled = true;
            RegisterButton.Text = "Crear Cuenta";

            if (success)
            {
                // Mostrar mensaje de éxito
                await DisplayAlert("✅ Éxito", 
                    "Cuenta creada exitosamente", 
                    "OK");

                // Navegar al AppShell principal
                Application.Current!.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("❌ Error", message, "OK");
            }
        }

        private async void OnLoginTapped(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnTogglePasswordClicked(object? sender, EventArgs e)
        {
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
            TogglePasswordButton.Text = PasswordEntry.IsPassword ? "👁" : "🙈";
        }

        private void OnToggleConfirmPasswordClicked(object? sender, EventArgs e)
        {
            ConfirmPasswordEntry.IsPassword = !ConfirmPasswordEntry.IsPassword;
            ToggleConfirmPasswordButton.Text = ConfirmPasswordEntry.IsPassword ? "👁" : "🙈";
        }
    }
}
