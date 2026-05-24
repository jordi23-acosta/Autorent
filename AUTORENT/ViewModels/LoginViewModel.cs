using AUTORENT.Services;
using System.Text.RegularExpressions;
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
        
        // Validation properties
        private string _emailError = string.Empty;
        private string _passwordError = string.Empty;
        private bool _hasEmailError;
        private bool _hasPasswordError;
        private bool _isEmailValid;
        private bool _isPasswordValid;
        private string _generalError = string.Empty;
        private bool _hasGeneralError;

        public LoginViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Iniciar Sesión";

            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
            TogglePasswordCommand = new RelayCommand(TogglePassword);
            NavigateToRegisterCommand = new AsyncRelayCommand(NavigateToRegisterAsync);
            ForgotPasswordCommand = new AsyncRelayCommand(ForgotPasswordAsync);
            DismissErrorCommand = new RelayCommand(DismissError);
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ValidateEmail();
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
                    ValidatePassword();
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

        // Validation Properties
        public string EmailError
        {
            get => _emailError;
            set
            {
                if (SetProperty(ref _emailError, value))
                {
                    HasEmailError = !string.IsNullOrEmpty(value);
                }
            }
        }

        public string PasswordError
        {
            get => _passwordError;
            set
            {
                if (SetProperty(ref _passwordError, value))
                {
                    HasPasswordError = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool HasEmailError
        {
            get => _hasEmailError;
            set => SetProperty(ref _hasEmailError, value);
        }

        public bool HasPasswordError
        {
            get => _hasPasswordError;
            set => SetProperty(ref _hasPasswordError, value);
        }

        public bool IsEmailValid
        {
            get => _isEmailValid;
            set => SetProperty(ref _isEmailValid, value);
        }

        public bool IsPasswordValid
        {
            get => _isPasswordValid;
            set => SetProperty(ref _isPasswordValid, value);
        }

        public string GeneralError
        {
            get => _generalError;
            set
            {
                if (SetProperty(ref _generalError, value))
                {
                    HasGeneralError = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool HasGeneralError
        {
            get => _hasGeneralError;
            set => SetProperty(ref _hasGeneralError, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand DismissErrorCommand { get; }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(_email))
            {
                EmailError = string.Empty;
                IsEmailValid = false;
                return;
            }

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(_email, emailPattern))
            {
                EmailError = "Ingresa un email válido";
                IsEmailValid = false;
                return;
            }

            // Verificar typos comunes en dominios
            var typoSuggestion = ErrorTranslator.CheckEmailTypo(_email);
            if (typoSuggestion != null)
            {
                EmailError = $"¿Quisiste decir @{typoSuggestion}?";
                IsEmailValid = false;
                return;
            }

            EmailError = string.Empty;
            IsEmailValid = true;
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(_password))
            {
                PasswordError = string.Empty;
                IsPasswordValid = false;
                return;
            }

            if (_password.Length < 6)
            {
                PasswordError = "Mínimo 6 caracteres";
                IsPasswordValid = false;
            }
            else
            {
                PasswordError = string.Empty;
                IsPasswordValid = true;
            }
        }

        private bool CanLogin()
        {
            return IsEmailValid && IsPasswordValid && !IsBusy;
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                GeneralError = string.Empty;

                var (success, message, user) = await _authService.LoginAsync(Email, Password);

                if (success && user != null)
                {
                    // Navegar a la página principal
                    Application.Current!.MainPage = new AppShell();
                }
                else
                {
                    // Si las credenciales son incorrectas, verificar typo en email
                    var emailTypo = ErrorTranslator.CheckEmailTypo(Email);
                    if (emailTypo != null && message.Contains("incorrect", StringComparison.OrdinalIgnoreCase) 
                        || (emailTypo != null && (message.Contains("incorrectos") || message.Contains("incorrecta"))))
                    {
                        var domain = Email.Split('@').LastOrDefault();
                        GeneralError = $"¿Quisiste decir @{emailTypo}? Verifica tu email";
                    }
                    else
                    {
                        GeneralError = message;
                    }
                }
            }
            catch (Exception ex)
            {
                GeneralError = ErrorTranslator.TranslateError(ex.Message);
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
            try
            {
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(new RegisterPage());
                }
                else if (Application.Current?.MainPage is Shell shell)
                {
                    await shell.GoToAsync("RegisterPage");
                }
                else
                {
                    Application.Current!.MainPage = new NavigationPage(new RegisterPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] Error navegando a Register: {ex.Message}");
                GeneralError = "Error al abrir el registro";
            }
        }

        private async Task ForgotPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                GeneralError = "Por favor ingresa tu email para recuperar tu contraseña";
                return;
            }

            try
            {
                IsBusy = true;
                var result = await _authService.ResetPasswordAsync(Email);

                if (result)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "✅ Email enviado",
                        "Se ha enviado un email con instrucciones para recuperar tu contraseña",
                        "OK");
                }
                else
                {
                    GeneralError = "No se pudo enviar el email de recuperación";
                }
            }
            catch (Exception ex)
            {
                GeneralError = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void DismissError()
        {
            GeneralError = string.Empty;
        }
    }
}
