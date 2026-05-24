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
        private string _userPhone = string.Empty;
        private string _userRole = string.Empty;
        private string _userInitials = "?";
        private string _memberSince = string.Empty;
        private bool _isDarkMode;
        private bool _isOwner;
        private int _totalRentals;
        private int _totalFavorites;
        private int _totalVehicles;

        public ProfileViewModel()
        {
            _authService = AuthService.Instance;
            Title = "Perfil";

            PersonalInfoCommand = new AsyncRelayCommand(EditPersonalInfoAsync);
            EditPhoneCommand = new AsyncRelayCommand(EditPhoneAsync);
            ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync);
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            HelpCommand = new AsyncRelayCommand(ShowHelpAsync);
            AboutCommand = new AsyncRelayCommand(ShowAboutAsync);

            // Cargar datos del usuario y estadísticas
            _ = LoadUserData();
            _ = LoadStatsAsync();
        }

        public string UserName
        {
            get => _userName;
            set
            {
                if (SetProperty(ref _userName, value))
                {
                    UpdateInitials();
                }
            }
        }

        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        public string UserPhone
        {
            get => _userPhone;
            set => SetProperty(ref _userPhone, value);
        }

        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        public string UserInitials
        {
            get => _userInitials;
            set => SetProperty(ref _userInitials, value);
        }

        public string MemberSince
        {
            get => _memberSince;
            set => SetProperty(ref _memberSince, value);
        }

        public bool IsOwner
        {
            get => _isOwner;
            set => SetProperty(ref _isOwner, value);
        }

        public int TotalRentals
        {
            get => _totalRentals;
            set => SetProperty(ref _totalRentals, value);
        }

        public int TotalFavorites
        {
            get => _totalFavorites;
            set => SetProperty(ref _totalFavorites, value);
        }

        public int TotalVehicles
        {
            get => _totalVehicles;
            set => SetProperty(ref _totalVehicles, value);
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (SetProperty(ref _isDarkMode, value))
                {
                    ThemeService.Instance.SetTheme(value);
                }
            }
        }

        public ICommand PersonalInfoCommand { get; }
        public ICommand EditPhoneCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand AboutCommand { get; }

        public async Task LoadUserData()
        {
            var user = _authService.CurrentUser;
            
            // Si no hay usuario o falta info, recargar desde Supabase
            if (user == null || string.IsNullOrWhiteSpace(user.Name))
            {
                await ReloadUserFromDatabaseAsync();
                user = _authService.CurrentUser;
            }
            
            if (user != null)
            {
                UserName = user.Name;
                UserEmail = user.Email;
                UserPhone = string.IsNullOrEmpty(user.Phone) ? "Sin teléfono" : user.Phone;
                IsOwner = user.Role == Models.UserRole.Owner;
                UserRole = IsOwner ? "Propietario" : "Conductor";
                MemberSince = $"Miembro desde {user.CreatedAt:MMMM yyyy}";
                UpdateInitials();
            }
        }

        private async Task ReloadUserFromDatabaseAsync()
        {
            try
            {
                var client = _authService.GetClient();
                if (client?.Auth.CurrentUser == null) return;

                var userId = client.Auth.CurrentUser.Id;

                var response = await client
                    .From<Profile>()
                    .Where(x => x.Id == userId)
                    .Single();

                if (response != null && _authService.CurrentUser != null)
                {
                    _authService.CurrentUser.Name = response.FullName;
                    _authService.CurrentUser.Email = response.Email;
                    _authService.CurrentUser.Phone = response.Phone ?? string.Empty;
                    _authService.CurrentUser.Role = response.UserType == "propietario" 
                        ? Models.UserRole.Owner 
                        : Models.UserRole.Driver;
                    _authService.CurrentUser.CreatedAt = response.CreatedAt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error recargando perfil: {ex.Message}");
            }
        }

        private void UpdateInitials()
        {
            if (string.IsNullOrWhiteSpace(_userName))
            {
                UserInitials = "?";
                return;
            }

            var parts = _userName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                UserInitials = $"{parts[0][0]}{parts[1][0]}".ToUpper();
            }
            else if (parts.Length == 1 && parts[0].Length > 0)
            {
                UserInitials = parts[0].Length >= 2 
                    ? parts[0].Substring(0, 2).ToUpper() 
                    : parts[0][0].ToString().ToUpper();
            }
            else
            {
                UserInitials = "?";
            }
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                var client = _authService.GetClient();
                var user = _authService.CurrentUser;
                if (client == null || user == null) return;

                // Cargar rentas del usuario
                var rentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.RenterId == user.Id)
                    .Get();
                TotalRentals = rentalsResponse?.Models?.Count ?? 0;

                // Si es propietario, cargar sus vehículos
                if (IsOwner)
                {
                    var vehiclesResponse = await client
                        .From<Vehicle>()
                        .Where(x => x.OwnerId == user.Id)
                        .Get();
                    TotalVehicles = vehiclesResponse?.Models?.Count ?? 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando estadísticas: {ex.Message}");
            }
        }

        private void LoadDarkModePreference()
        {
            _isDarkMode = ThemeService.Instance.IsDarkMode();
            OnPropertyChanged(nameof(IsDarkMode));
        }

        private void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
        }

        private async Task EditPersonalInfoAsync()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            string? name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "✏️ Editar Nombre",
                "Ingresa tu nombre completo:",
                initialValue: user.Name,
                maxLength: 100,
                keyboard: Microsoft.Maui.Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(name) && name != user.Name)
            {
                try
                {
                    IsBusy = true;
                    var client = _authService.GetClient();
                    if (client != null)
                    {
                        await client
                            .From<Profile>()
                            .Where(x => x.Id == user.Id)
                            .Set(x => x.FullName, name)
                            .Set(x => x.UpdatedAt, DateTime.UtcNow)
                            .Update();

                        user.Name = name;
                        UserName = name;

                        await Application.Current!.MainPage!.DisplayAlert(
                            "✅ Actualizado", 
                            "Tu nombre ha sido actualizado", 
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error actualizando perfil: {ex.Message}");
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error", 
                        ErrorTranslator.TranslateError(ex.Message), 
                        "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task EditPhoneAsync()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            string? phone = await Application.Current!.MainPage!.DisplayPromptAsync(
                "📱 Editar Teléfono",
                "Ingresa tu número de teléfono:",
                initialValue: user.Phone ?? string.Empty,
                maxLength: 20,
                keyboard: Microsoft.Maui.Keyboard.Telephone);

            if (!string.IsNullOrWhiteSpace(phone) && phone != user.Phone)
            {
                try
                {
                    IsBusy = true;
                    var client = _authService.GetClient();
                    if (client != null)
                    {
                        await client
                            .From<Profile>()
                            .Where(x => x.Id == user.Id)
                            .Set(x => x.Phone!, phone)
                            .Set(x => x.UpdatedAt, DateTime.UtcNow)
                            .Update();

                        user.Phone = phone;
                        UserPhone = phone;

                        await Application.Current!.MainPage!.DisplayAlert(
                            "✅ Actualizado", 
                            "Tu teléfono ha sido actualizado", 
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error", 
                        ErrorTranslator.TranslateError(ex.Message), 
                        "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task ChangePasswordAsync()
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "🔒 Cambiar Contraseña",
                "Te enviaremos un email con instrucciones para cambiar tu contraseña",
                "OK");

            try
            {
                var user = _authService.CurrentUser;
                if (user != null)
                {
                    IsBusy = true;
                    var result = await _authService.ResetPasswordAsync(user.Email);
                    
                    await Application.Current!.MainPage!.DisplayAlert(
                        result ? "✅ Email enviado" : "Error",
                        result 
                            ? "Revisa tu email para cambiar tu contraseña" 
                            : "No se pudo enviar el email",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    ErrorTranslator.TranslateError(ex.Message),
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowHelpAsync()
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "💬 Ayuda y Soporte",
                "Para ayuda contacta:\n\n📧 soporte@autorent.com\n📱 +52 555 123 4567\n\nHorario: Lun-Vie 9am-6pm",
                "OK");
        }

        private async Task ShowAboutAsync()
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "ℹ️ Acerca de AUTORENT",
                "AUTORENT v1.0\n\nLa mejor plataforma para rentar y publicar autos.\n\n© 2026 AUTORENT",
                "OK");
        }

        private async Task LogoutAsync()
        {
            bool answer = await Application.Current!.MainPage!.DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro que deseas cerrar sesión?", 
                "Sí, cerrar", 
                "Cancelar");

            if (answer)
            {
                _authService.Logout();
                Application.Current!.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
