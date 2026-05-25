using Supabase;
using SupabaseUser = Supabase.Gotrue.User;
using AUTORENT.Models;

namespace AUTORENT.Services
{
    public class AuthService
    {
        private static AuthService? _instance;
        private Supabase.Client? _supabaseClient;
        private User? _currentUser;

        public static AuthService Instance => _instance ??= new AuthService();

        public User? CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;

        private AuthService() { }

        public async Task InitializeAsync(string supabaseUrl, string supabaseKey)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[AUTH] 🚀 Iniciando Supabase...");
                
                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = true,
                    SessionHandler = new SupabaseSessionHandler() // Persistir sesión
                };

                _supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, options);
                await _supabaseClient.InitializeAsync();

                System.Diagnostics.Debug.WriteLine("[AUTH] ✅ Supabase inicializado");

                // Verificar si hay una sesión activa guardada
                var session = _supabaseClient.Auth.CurrentSession;
                if (session != null && session.User != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[AUTH] 🔑 Sesión encontrada para: {session.User.Email}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] 👤 User ID: {session.User.Id}");
                    
                    await LoadCurrentUserProfile();
                    
                    if (_currentUser != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AUTH] ✅ Usuario restaurado: {_currentUser.Name} ({_currentUser.Email})");
                        System.Diagnostics.Debug.WriteLine($"[AUTH] 🎭 Rol: {_currentUser.Role}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[AUTH] ⚠️ No se pudo cargar el perfil del usuario");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[AUTH] ℹ️ No hay sesión guardada");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] ❌ Error inicializando Supabase: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AUTH] Stack: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<(bool success, string message, User? user)> LoginAsync(string email, string password)
        {
            try
            {
                if (_supabaseClient == null)
                    return (false, "Servicio no inicializado", null);

                System.Diagnostics.Debug.WriteLine($"[AUTH] 🔐 Intentando login para: {email}");
                
                var session = await _supabaseClient.Auth.SignIn(email, password);

                if (session?.User == null)
                {
                    System.Diagnostics.Debug.WriteLine("[AUTH] ❌ Login fallido - sin usuario");
                    return (false, "Error al iniciar sesión", null);
                }

                System.Diagnostics.Debug.WriteLine($"[AUTH] ✅ Login exitoso para: {session.User.Email}");
                System.Diagnostics.Debug.WriteLine($"[AUTH] 👤 User ID: {session.User.Id}");
                System.Diagnostics.Debug.WriteLine($"[AUTH] 🔑 AccessToken: {session.AccessToken?.Substring(0, 20)}...");
                
                await LoadCurrentUserProfile();
                
                if (_currentUser != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[AUTH] ✅ Perfil cargado: {_currentUser.Name}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] 🎭 Rol: {_currentUser.Role}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] 💾 Sesión guardada automáticamente por SessionHandler");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[AUTH] ⚠️ No se pudo cargar el perfil");
                }

                return (true, "Inicio de sesión exitoso", _currentUser);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] ❌ Error en login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AUTH] Stack: {ex.StackTrace}");
                return (false, ErrorTranslator.TranslateError(ex.Message), null);
            }
        }

        public async Task<(bool success, string message, User? user)> RegisterAsync(
            string email, 
            string password, 
            string fullName, 
            string phone, 
            string userType)
        {
            try
            {
                if (_supabaseClient == null)
                    return (false, "Servicio no inicializado", null);

                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Iniciando registro para: {email}");

                // Crear usuario en Supabase Auth con todos los datos
                var options = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "full_name", fullName },
                        { "phone", phone },
                        { "user_type", userType }
                    }
                };

                var session = await _supabaseClient.Auth.SignUp(email, password, options);

                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Usuario creado en Auth. ID: {session?.User?.Id}");
                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Email confirmado: {session?.User?.EmailConfirmedAt != null}");

                if (session?.User == null)
                    return (false, "Error al crear la cuenta", null);

                // Esperar para que el trigger cree el perfil
                await Task.Delay(2000);

                // Estrategia: Intentar actualizar primero, si falla por que no existe, crearlo
                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Guardando teléfono: '{phone}', tipo: '{userType}'");
                
                bool profileSaved = false;
                
                // Intento 1: Actualizar el perfil que el trigger debió crear
                try
                {
                    var updateResponse = await _supabaseClient
                        .From<Profile>()
                        .Where(x => x.Id == session.User.Id)
                        .Set(x => x.Phone, phone)
                        .Set(x => x.UserType, userType)
                        .Set(x => x.FullName, fullName)
                        .Set(x => x.UpdatedAt, DateTime.UtcNow)
                        .Update();

                    if (updateResponse?.Models?.Count > 0)
                    {
                        var updated = updateResponse.Models[0];
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] ✅ Perfil actualizado");
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] Teléfono guardado: '{updated.Phone}'");
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] Tipo guardado: '{updated.UserType}'");
                        profileSaved = !string.IsNullOrEmpty(updated.Phone);
                    }
                }
                catch (Exception updateEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] Error en update: {updateEx.Message}");
                }

                // Intento 2: Si el update no guardó el teléfono, intentar insert
                if (!profileSaved)
                {
                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] Update no guardó teléfono, intentando INSERT...");
                    try
                    {
                        var newProfile = new Profile
                        {
                            Id = session.User.Id,
                            Email = email,
                            FullName = fullName,
                            Phone = phone,
                            UserType = userType,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _supabaseClient.From<Profile>().Upsert(newProfile);
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] ✅ Perfil creado/actualizado con UPSERT");
                        profileSaved = true;
                    }
                    catch (Exception insertEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] Error en upsert: {insertEx.Message}");
                        
                        // Último intento: Update sin filtros estrictos
                        try
                        {
                            await _supabaseClient
                                .From<Profile>()
                                .Where(x => x.Id == session.User.Id)
                                .Set(x => x.Phone, phone)
                                .Update();
                            System.Diagnostics.Debug.WriteLine($"[REGISTRO] ✅ Teléfono guardado en último intento");
                        }
                        catch (Exception lastEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"[REGISTRO] ❌ Falló todo: {lastEx.Message}");
                        }
                    }
                }

                await LoadCurrentUserProfile();

                // Si LoadCurrentUserProfile no cargó el usuario por timing,
                // asignarlo manualmente con los datos que ya tenemos
                if (_currentUser == null && session.User != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] LoadCurrentUserProfile no cargó usuario, asignando manualmente");
                    _currentUser = new User
                    {
                        Id = session.User.Id,
                        Name = fullName,
                        Email = email,
                        Phone = phone,
                        Role = userType == "propietario" ? UserRole.Owner : UserRole.Driver,
                        CreatedAt = DateTime.UtcNow,
                        IsVerified = true
                    };
                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] ✅ Usuario asignado manualmente con rol: {_currentUser.Role}");
                }
                else if (_currentUser != null)
                {
                    // Forzar el rol correcto según el registro
                    var expectedRole = userType == "propietario" ? UserRole.Owner : UserRole.Driver;
                    if (_currentUser.Role != expectedRole)
                    {
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] ⚠️ Rol incorrecto detectado, corrigiendo de {_currentUser.Role} a {expectedRole}");
                        _currentUser.Role = expectedRole;
                    }
                }

                string successMessage = session.User.EmailConfirmedAt == null 
                    ? "Cuenta creada. Por favor verifica tu email para continuar." 
                    : "Cuenta creada exitosamente";

                return (true, successMessage, _currentUser);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Error general: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Stack trace: {ex.StackTrace}");
                return (false, ErrorTranslator.TranslateError(ex.Message), null);
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                if (_supabaseClient == null)
                    return false;

                System.Diagnostics.Debug.WriteLine("[AUTH] 🚪 Cerrando sesión...");
                System.Diagnostics.Debug.WriteLine($"[AUTH] Usuario actual: {_currentUser?.Email}");
                
                await _supabaseClient.Auth.SignOut();
                _currentUser = null;
                
                System.Diagnostics.Debug.WriteLine("[AUTH] ✅ Sesión cerrada exitosamente");
                System.Diagnostics.Debug.WriteLine("[AUTH] 🗑️ Sesión eliminada de Preferences");
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] ❌ Error al cerrar sesión: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                if (_supabaseClient == null)
                    return false;

                await _supabaseClient.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al recuperar contraseña: {ex.Message}");
                return false;
            }
        }

        private async Task LoadCurrentUserProfile()
        {
            try
            {
                if (_supabaseClient?.Auth.CurrentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine("[AUTH] ⚠️ No hay usuario actual para cargar perfil");
                    return;
                }

                var userId = _supabaseClient.Auth.CurrentUser.Id;
                System.Diagnostics.Debug.WriteLine($"[AUTH] 📥 Cargando perfil para User ID: {userId}");
                
                var response = await _supabaseClient
                    .From<Profile>()
                    .Where(x => x.Id == userId)
                    .Single();

                if (response != null)
                {
                    _currentUser = new User
                    {
                        Id = response.Id,
                        Name = response.FullName,
                        Email = response.Email,
                        Phone = response.Phone ?? string.Empty,
                        Role = response.UserType == "propietario" ? UserRole.Owner : UserRole.Driver,
                        ProfileImage = response.AvatarUrl,
                        CreatedAt = response.CreatedAt,
                        IsVerified = true
                    };
                    
                    System.Diagnostics.Debug.WriteLine($"[AUTH] ✅ Perfil cargado exitosamente");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] 👤 Nombre: {_currentUser.Name}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] 📧 Email: {_currentUser.Email}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] 🎭 Rol: {_currentUser.Role}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[AUTH] ⚠️ No se encontró perfil en la base de datos");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] ❌ Error cargando perfil: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AUTH] Stack: {ex.StackTrace}");
            }
        }

        public Supabase.Client? GetClient() => _supabaseClient;

        public bool IsOwner()
        {
            return _currentUser?.Role == UserRole.Owner;
        }

        public void Logout()
        {
            _ = LogoutAsync();
        }
    }
}
