using AUTORENT.Services;
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

        public RegisterViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Registro";

            RegisterCommand = new AsyncRelayCommand(RegisterAsync, CanRegister);
            TogglePasswordCommand = new RelayCommand(TogglePassword);
            ToggleConfirmPasswordCommand = new RelayCommand(ToggleConfirmPassword);
            NavigateToLoginCommand = new AsyncRelayCommand(NavigateToLoginAsync);
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (SetProperty(ref _phone, value))
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                    ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
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

        public ICommand RegisterCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   SelectedUserTypeIndex != -1 &&
                   !IsBusy;
        }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            // Validaciones
            if (Password != ConfirmPassword)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error", 
                    "Las contraseñas no coinciden", 
                    "OK");
                return;
            }

            if (Password.Length < 6)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error", 
                    "La contraseña debe tener al menos 6 caracteres", 
                    "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Determinar tipo de usuario
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
                        "✅ Éxito",
                        "Cuenta creada exitosamente",
                        "OK");

                    // Navegar al AppShell principal
                    Application.Current!.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "❌ Error", 
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

        private void ToggleConfirmPassword()
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
            ConfirmPasswordToggleIcon = IsConfirmPasswordVisible ? "🙈" : "👁";
        }

        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
