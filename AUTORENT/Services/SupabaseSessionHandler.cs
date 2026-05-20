using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System.Text.Json;

namespace AUTORENT.Services
{
    public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
    {
        private const string SessionKey = "supabase.session";

        public void SaveSession(Session session)
        {
            try
            {
                if (session == null)
                {
                    Preferences.Remove(SessionKey);
                    System.Diagnostics.Debug.WriteLine("[SESSION] ❌ Sesión eliminada");
                    return;
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNameCaseInsensitive = true
                };

                var sessionJson = JsonSerializer.Serialize(session, options);
                Preferences.Set(SessionKey, sessionJson);
                
                var expiresAt = session.ExpiresAt();
                System.Diagnostics.Debug.WriteLine($"[SESSION] ✅ Sesión guardada para: {session.User?.Email}");
                System.Diagnostics.Debug.WriteLine($"[SESSION] 📝 AccessToken: {session.AccessToken?.Substring(0, 20)}...");
                System.Diagnostics.Debug.WriteLine($"[SESSION] ⏰ Expira: {expiresAt}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SESSION] ❌ Error guardando sesión: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SESSION] Stack: {ex.StackTrace}");
            }
        }

        public void DestroySession()
        {
            try
            {
                Preferences.Remove(SessionKey);
                System.Diagnostics.Debug.WriteLine("[SESSION] 🗑️ Sesión destruida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SESSION] ❌ Error destruyendo sesión: {ex.Message}");
            }
        }

        public Session? LoadSession()
        {
            try
            {
                var sessionJson = Preferences.Get(SessionKey, null);
                if (string.IsNullOrEmpty(sessionJson))
                {
                    System.Diagnostics.Debug.WriteLine("[SESSION] ℹ️ No hay sesión guardada");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var session = JsonSerializer.Deserialize<Session>(sessionJson, options);
                
                if (session != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[SESSION] ✅ Sesión cargada para: {session.User?.Email}");
                    System.Diagnostics.Debug.WriteLine($"[SESSION] 📝 AccessToken: {session.AccessToken?.Substring(0, 20)}...");
                    
                    // Verificar si la sesión ha expirado
                    var expiresAt = session.ExpiresAt();
                    if (expiresAt < DateTimeOffset.UtcNow)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SESSION] ⚠️ Sesión expirada ({expiresAt}), eliminando...");
                        Preferences.Remove(SessionKey);
                        return null;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"[SESSION] ⏰ Expira: {expiresAt}");
                }
                
                return session;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SESSION] ❌ Error cargando sesión: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SESSION] Stack: {ex.StackTrace}");
                
                // Si hay error al cargar, limpiar la sesión corrupta
                Preferences.Remove(SessionKey);
                return null;
            }
        }
    }
}
