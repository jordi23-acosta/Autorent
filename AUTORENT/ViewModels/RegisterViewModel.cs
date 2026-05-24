using AUTORENT.Services;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isPasswordVisible;
        private bool _isConfirmPasswordVisible;
        private string _passwordToggleIcon = "👁";
        private string _confirmPasswordToggleIcon = "👁";
        private int _selectedUserTypeIndex = -1;

        // Validation properties
        private string _nameError = string.Empty;
        private string _emailError = string.Empty;
        private string _phoneError = string.Empty;
        private string _passwordError = string.Empty;
        private string _confirmPasswordError = string.Empty;
        private bool _isNameValid;
        private bool _isEmailValid;
        private bool _isPhoneValid;
        private bool _isPasswordValid;
        private bool _isConfirmPasswordValid;
        private bool _hasNameError;
        private bool _hasEmailError;
        private bool _hasPhoneError;
        private bool _hasPasswordError;
        private bool _hasConfirmPasswordError;
        private string _generalError = string.Empty;
        private bool _hasGeneralError;
        private string _passwordStrength = string.Empty;
        private double _passwordStrengthValue;
        private Color _passwordStrengthColor = Colors.Gray;

        public RegisterViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Registro";

            RegisterCommand = new AsyncRelayCommand(RegisterAsync, CanRegister);
            TogglePasswordCommand = new RelayCommand(TogglePassword);
            ToggleConfirmPasswordCommand = new RelayCommand(ToggleConfirmPassword);
            NavigateToLoginCommand = new AsyncRelayCommand(NavigateToLoginAsync);
            DismissErrorCommand = new RelayCommand(DismissError);
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    ValidateName();
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ValidateEmail();
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (SetProperty(ref _phone, value))
                {
                    ValidatePhone();
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
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
                    if (!string.IsNullOrEmpty(_confirmPassword))
                        ValidateConfirmPassword();
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                {
                    ValidateConfirmPassword();
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set => SetProperty(ref _isPasswordVisible, value);
        }

        public bool IsConfirmPasswordVisible
        {
            get => _isConfirmPasswordVisible;
            set => SetProperty(ref _isConfirmPasswordVisible, value);
        }

        public string PasswordToggleIcon
        {
            get => _passwordToggleIcon;
            set => SetProperty(ref _passwordToggleIcon, value);
        }

        public string ConfirmPasswordToggleIcon
        {
            get => _confirmPasswordToggleIcon;
            set => SetProperty(ref _confirmPasswordToggleIcon, value);
        }

        public int SelectedUserTypeIndex
        {
            get => _selectedUserTypeIndex;
            set
            {
                if (SetProperty(ref _selectedUserTypeIndex, value))
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        // Validation Properties
        public string NameError
        {
            get => _nameError;
            set
            {
                if (SetProperty(ref _nameError, value))
                    HasNameError = !string.IsNullOrEmpty(value);
            }
        }

        public string EmailError
        {
            get => _emailError;
            set
            {
                if (SetProperty(ref _emailError, value))
                    HasEmailError = !string.IsNullOrEmpty(value);
            }
        }

        public string PhoneError
        {
            get => _phoneError;
            set
            {
                if (SetProperty(ref _phoneError, value))
                    HasPhoneError = !string.IsNullOrEmpty(value);
            }
        }

        public string PasswordError
        {
            get => _passwordError;
            set
            {
                if (SetProperty(ref _passwordError, value))
                    HasPasswordError = !string.IsNullOrEmpty(value);
            }
        }

        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set
            {
                if (SetProperty(ref _confirmPasswordError, value))
                    HasConfirmPasswordError = !string.IsNullOrEmpty(value);
            }
        }

        public bool IsNameValid { get => _isNameValid; set => SetProperty(ref _isNameValid, value); }
        public bool IsEmailValid { get => _isEmailValid; set => SetProperty(ref _isEmailValid, value); }
        public bool IsPhoneValid { get => _isPhoneValid; set => SetProperty(ref _isPhoneValid, value); }
        public bool IsPasswordValid { get => _isPasswordValid; set => SetProperty(ref _isPasswordValid, value); }
        public bool IsConfirmPasswordValid { get => _isConfirmPasswordValid; set => SetProperty(ref _isConfirmPasswordValid, value); }

        public bool HasNameError { get => _hasNameError; set => SetProperty(ref _hasNameError, value); }
        public bool HasEmailError { get => _hasEmailError; set => SetProperty(ref _hasEmailError, value); }
        public bool HasPhoneError { get => _hasPhoneError; set => SetProperty(ref _hasPhoneError, value); }
        public bool HasPasswordError { get => _hasPasswordError; set => SetProperty(ref _hasPasswordError, value); }
        public bool HasConfirmPasswordError { get => _hasConfirmPasswordError; set => SetProperty(ref _hasConfirmPasswordError, value); }

        public string GeneralError
        {
            get => _generalError;
            set
            {
                if (SetProperty(ref _generalError, value))
                    HasGeneralError = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasGeneralError { get => _hasGeneralError; set => SetProperty(ref _hasGeneralError, value); }

        public string PasswordStrength
        {
            get => _passwordStrength;
            set => SetProperty(ref _passwordStrength, value);
        }

        public double PasswordStrengthValue
        {
            get => _passwordStrengthValue;
            set => SetProperty(ref _passwordStrengthValue, value);
        }

        public Color PasswordStrengthColor
        {
            get => _passwordStrengthColor;
            set => SetProperty(ref _passwordStrengthColor, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand DismissErrorCommand { get; }

        private void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                NameError = string.Empty;
                IsNameValid = false;
                return;
            }

            if (_name.Trim().Length < 3)
            {
                NameError = "Mínimo 3 caracteres";
                IsNameValid = false;
            }
            else
            {
                NameError = string.Empty;
                IsNameValid = true;
            }
        }

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
            }
            else
            {
                EmailError = string.Empty;
                IsEmailValid = true;
            }
        }

        private void ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(_phone))
            {
                PhoneError = string.Empty;
                IsPhoneValid = false;
                return;
            }

            var digitsOnly = Regex.Replace(_phone, @"\D", "");
            if (digitsOnly.Length < 10)
            {
                PhoneError = "Mínimo 10 dígitos";
                IsPhoneValid = false;
            }
            else
            {
                PhoneError = string.Empty;
                IsPhoneValid = true;
            }
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(_password))
            {
                PasswordError = string.Empty;
                IsPasswordValid = false;
                PasswordStrength = string.Empty;
                PasswordStrengthValue = 0;
                return;
            }

            // Calcular fortaleza
            int score = 0;
            if (_password.Length >= 6) score++;
            if (_password.Length >= 10) score++;
            if (Regex.IsMatch(_password, @"[A-Z]")) score++;
            if (Regex.IsMatch(_password, @"[0-9]")) score++;
            if (Regex.IsMatch(_password, @"[^A-Za-z0-9]")) score++;

            PasswordStrengthValue = score / 5.0;

            switch (score)
            {
                case 0:
                case 1:
                    PasswordStrength = "Débil";
                    PasswordStrengthColor = Color.FromArgb("#F44336");
                    break;
                case 2:
                case 3:
                    PasswordStrength = "Media";
                    PasswordStrengthColor = Color.FromArgb("#FF9800");
                    break;
                case 4:
                    PasswordStrength = "Buena";
                    PasswordStrengthColor = Color.FromArgb("#2196F3");
                    break;
                case 5:
                    PasswordStrength = "Excelente";
                    PasswordStrengthColor = Color.FromArgb("#4CAF50");
                    break;
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

        private void ValidateConfirmPassword()
        {
            if (string.IsNullOrWhiteSpace(_confirmPassword))
            {
                ConfirmPasswordError = string.Empty;
                IsConfirmPasswordValid = false;
                return;
            }

            if (_confirmPassword != _password)
            {
                ConfirmPasswordError = "Las contraseñas no coinciden";
                IsConfirmPasswordValid = false;
            }
            else
            {
                ConfirmPasswordError = string.Empty;
                IsConfirmPasswordValid = true;
            }
        }

        private bool CanRegister()
        {
            return IsNameValid && IsEmailValid && IsPhoneValid && 
                   IsPasswordValid && IsConfirmPasswordValid &&
                   SelectedUserTypeIndex != -1 && !IsBusy;
        }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                GeneralError = string.Empty;

                string userType = SelectedUserTypeIndex == 0 ? "conductor" : "propietario";

                var (success, message, user) = await _authService.RegisterAsync(
                    Email.Trim(),
                    Password,
                    Name.Trim(),
                    Phone.Trim(),
                    userType
                );

                if (success)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "✅ ¡Bienvenido!",
                        "Cuenta creada exitosamente",
                        "OK");

                    Application.Current!.MainPage = new AppShell();
                }
                else
                {
                    GeneralError = message;
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

        private void TogglePassword()
        {
            IsPasswordVisible = !IsPasswordVisible;
            PasswordToggleIcon = IsPasswordVisible ? "🙈" : "👁";
        }

        private void ToggleConfirmPassword()
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
            ConfirmPasswordToggleIcon = IsConfirmPasswordVisible ? "🙈" : "👁";
        }

        private async Task NavigateToLoginAsync()
        {
            try
            {
                if (Application.Current?.MainPage is NavigationPage navPage && navPage.Navigation.NavigationStack.Count > 1)
                {
                    await navPage.PopAsync();
                }
                else
                {
                    Application.Current!.MainPage = new NavigationPage(new LoginPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[REGISTER] Error volviendo a Login: {ex.Message}");
                Application.Current!.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private void DismissError()
        {
            GeneralError = string.Empty;
        }
    }
}
