using AUTORENT.Services;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isPasswordVisible;
        private string _passwordToggleIcon = "👁";

        public LoginViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Iniciar Sesión";

            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
            TogglePasswordCommand = new RelayCommand(TogglePassword);
            NavigateToRegisterCommand = new AsyncRelayCommand(NavigateToRegisterAsync);
            ForgotPasswordCommand = new AsyncRelayCommand(ForgotPasswordAsync);
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set => SetProperty(ref _isPasswordVisible, value);
        }

        public string PasswordToggleIcon
        {
            get => _passwordToggleIcon;
            set => SetProperty(ref _passwordToggleIcon, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && 
                   !string.IsNullOrWhiteSpace(Password) && 
                   !IsBusy;
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var (success, message, user) = await _authService.LoginAsync(Email, Password);

                if (success && user != null)
                {
                    // Navegar a la página principal
                    Application.Current!.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error de inicio de sesión",
                        message,
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Ocurrió un error: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void TogglePassword()
        {
            IsPasswordVisible = !IsPasswordVisible;
            PasswordToggleIcon = IsPasswordVisible ? "🙈" : "👁";
        }

        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        private async Task ForgotPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Email requerido",
                    "Por favor ingresa tu email para recuperar tu contraseña",
                    "OK");
                return;
            }

            var result = await _authService.ResetPasswordAsync(Email);
            
            if (result)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Email enviado",
                    "Se ha enviado un email con instrucciones para recuperar tu contraseña",
                    "OK");
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    "No se pudo enviar el email de recuperación",
                    "OK");
            }
        }
    }
}
