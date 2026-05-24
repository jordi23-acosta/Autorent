namespace AUTORENT.Services
{
    /// <summary>
    /// Traduce errores técnicos de Supabase y otros servicios
    /// a mensajes amigables para el usuario en español.
    /// </summary>
    public static class ErrorTranslator
    {
        public static string TranslateError(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                return "Ha ocurrido un error inesperado";

            var lowerError = errorMessage.ToLower();

            // Errores de credenciales
            if (lowerError.Contains("invalid_credentials") || 
                lowerError.Contains("invalid login credentials"))
                return "El email o la contraseña son incorrectos";

            // Email no confirmado
            if (lowerError.Contains("email not confirmed") ||
                lowerError.Contains("email_not_confirmed"))
                return "Debes confirmar tu email antes de iniciar sesión";

            // Usuario ya registrado
            if (lowerError.Contains("user already registered") ||
                lowerError.Contains("already registered") ||
                lowerError.Contains("user_already_exists"))
                return "Este email ya está registrado. Intenta iniciar sesión";

            // Email inválido
            if (lowerError.Contains("invalid email") ||
                lowerError.Contains("invalid_email"))
                return "El formato del email no es válido";

            // Contraseña débil
            if (lowerError.Contains("password should be at least") ||
                lowerError.Contains("weak_password") ||
                lowerError.Contains("password is too short"))
                return "La contraseña debe tener al menos 6 caracteres";

            // Demasiados intentos
            if (lowerError.Contains("too many requests") ||
                lowerError.Contains("rate limit") ||
                lowerError.Contains("too_many_requests"))
                return "Demasiados intentos. Espera un momento e intenta de nuevo";

            // Errores de red
            if (lowerError.Contains("network") ||
                lowerError.Contains("connection") ||
                lowerError.Contains("timeout") ||
                lowerError.Contains("unable to resolve"))
                return "Error de conexión. Verifica tu internet";

            // Token expirado
            if (lowerError.Contains("token") && lowerError.Contains("expired"))
                return "Tu sesión ha expirado. Inicia sesión de nuevo";

            // Usuario no encontrado
            if (lowerError.Contains("user not found") ||
                lowerError.Contains("user_not_found"))
                return "Usuario no encontrado";

            // Permisos
            if (lowerError.Contains("permission denied") ||
                lowerError.Contains("not authorized") ||
                lowerError.Contains("unauthorized"))
                return "No tienes permisos para realizar esta acción";

            // Validación
            if (lowerError.Contains("validation") || lowerError.Contains("required"))
                return "Por favor completa todos los campos requeridos";

            // Error 400/401/403/404/500
            if (lowerError.Contains("\"code\":400") || lowerError.Contains("400 bad request"))
                return "Los datos ingresados no son válidos";

            if (lowerError.Contains("\"code\":401") || lowerError.Contains("401 unauthorized"))
                return "Credenciales incorrectas o sesión expirada";

            if (lowerError.Contains("\"code\":403") || lowerError.Contains("403 forbidden"))
                return "No tienes permisos para esta acción";

            if (lowerError.Contains("\"code\":404") || lowerError.Contains("404 not found"))
                return "El recurso solicitado no existe";

            if (lowerError.Contains("\"code\":500") || lowerError.Contains("500 internal"))
                return "Error del servidor. Intenta más tarde";

            // Si el mensaje contiene JSON, extraer solo el msg
            if (errorMessage.Contains("\"msg\""))
            {
                try
                {
                    var msgStart = errorMessage.IndexOf("\"msg\":\"") + 7;
                    var msgEnd = errorMessage.IndexOf("\"", msgStart);
                    if (msgStart > 6 && msgEnd > msgStart)
                    {
                        var extracted = errorMessage.Substring(msgStart, msgEnd - msgStart);
                        return TranslateError(extracted);
                    }
                }
                catch { }
            }

            // Si nada coincide, devolver mensaje genérico amigable
            return "No se pudo completar la operación. Intenta de nuevo";
        }
    }
}
