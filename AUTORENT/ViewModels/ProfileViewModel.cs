using AUTORENT.Services;
using AUTORENT.Models;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        private string _userName = string.Empty;
        private string _userEmail = string.Empty;
        private string _userRole = string.Empty;
        private bool _isDarkMode;

        public ProfileViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Perfil";

            PersonalInfoCommand = new AsyncRelayCommand(EditPersonalInfoAsync);
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);

            // Suscribirse a cambios de tema
            ThemeService.Instance.ThemeChanged += (s, theme) => OnPropertyChanged(nameof(IsDarkMode));

            LoadUserData();
            LoadDarkModePreference();
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (SetProperty(ref _isDarkMode, value))
                {
                    ThemeService.Instance.SetTheme(value);
                    string message = value ? "Modo oscuro activado" : "Modo claro activado";
                    Application.Current?.MainPage?.DisplayAlert("Tema", message, "OK");
                }
            }
        }

        public ICommand PersonalInfoCommand { get; }
        public ICommand LogoutCommand { get; }

        public void LoadUserData()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                UserName = user.Name;
                UserEmail = user.Email;
                UserRole = user.Role == Models.UserRole.Driver ? "Conductor" : "Propietario";
            }
        }

        private void LoadDarkModePreference()
        {
            _isDarkMode = ThemeService.Instance.IsDarkMode();
            OnPropertyChanged(nameof(IsDarkMode));
        }

        private async Task EditPersonalInfoAsync()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            // Mostrar información personal editable
            string? name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Nombre Completo",
                "Ingresa tu nombre completo:",
                initialValue: user.Name,
                maxLength: 100,
                keyboard: Microsoft.Maui.Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(name) && name != user.Name)
            {
                try
                {
                    var client = _authService.GetClient();
                    if (client != null)
                    {
                        // Actualizar en Supabase
                        await client
                            .From<Profile>()
                            .Where(x => x.Id == user.Id)
                            .Set(x => x.FullName, name)
                            .Set(x => x.UpdatedAt, DateTime.UtcNow)
                            .Update();

                        // Actualizar localmente
                        user.Name = name;
                        UserName = name;

                        await Application.Current!.MainPage!.DisplayAlert(
                            "✅ Éxito", 
                            "Tu información ha sido actualizada", 
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error actualizando perfil: {ex.Message}");
                    await Application.Current!.MainPage!.DisplayAlert(
                        "❌ Error", 
                        "No se pudo actualizar la información", 
                        "OK");
                }
            }
        }

        private async Task LogoutAsync()
        {
            bool answer = await Application.Current!.MainPage!.DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro que deseas cerrar sesión?", 
                "Sí", 
                "No");

            if (answer)
            {
                _authService.Logout();
                Application.Current!.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
