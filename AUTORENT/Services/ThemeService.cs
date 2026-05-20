using Microsoft.Maui.Controls;

namespace AUTORENT.Services
{
    public class ThemeService
    {
        private static ThemeService? _instance;
        public static ThemeService Instance => _instance ??= new ThemeService();

        public event EventHandler<AppTheme>? ThemeChanged;

        private ThemeService()
        {
            // Cargar tema guardado al iniciar
            LoadSavedTheme();
        }

        public void LoadSavedTheme()
        {
            bool isDarkMode = Preferences.Get("DarkMode", false);
            ApplyTheme(isDarkMode ? AppTheme.Dark : AppTheme.Light);
        }

        public void ToggleTheme()
        {
            bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
            SetTheme(!isDarkMode);
        }

        public void SetTheme(bool isDarkMode)
        {
            // Guardar preferencia
            Preferences.Set("DarkMode", isDarkMode);

            // Aplicar tema
            ApplyTheme(isDarkMode ? AppTheme.Dark : AppTheme.Light);
        }

        private void ApplyTheme(AppTheme theme)
        {
            if (Application.Current == null) return;

            // Cambiar tema de la aplicación
            Application.Current.UserAppTheme = theme;

            // Actualizar colores dinámicos
            UpdateDynamicColors(theme == AppTheme.Dark);

            // Notificar cambio de tema
            ThemeChanged?.Invoke(this, theme);
        }

        private void UpdateDynamicColors(bool isDarkMode)
        {
            if (Application.Current?.Resources == null) return;

            var resources = Application.Current.Resources;

            if (isDarkMode)
            {
                // Aplicar colores oscuros
                resources["PageBackground"] = resources["PageBackgroundDark"];
                resources["CardBackground"] = resources["CardBackgroundDark"];
                resources["TextPrimary"] = resources["TextPrimaryDark"];
                resources["TextSecondary"] = resources["TextSecondaryDark"];
                resources["Divider"] = resources["DividerDark"];
                resources["TabBarBackground"] = resources["TabBarBackgroundDark"];
            }
            else
            {
                // Aplicar colores claros
                resources["PageBackground"] = resources["PageBackgroundLight"];
                resources["CardBackground"] = resources["CardBackgroundLight"];
                resources["TextPrimary"] = resources["TextPrimaryLight"];
                resources["TextSecondary"] = resources["TextSecondaryLight"];
                resources["Divider"] = resources["DividerLight"];
                resources["TabBarBackground"] = resources["TabBarBackgroundLight"];
            }
        }

        public bool IsDarkMode()
        {
            return Application.Current?.RequestedTheme == AppTheme.Dark;
        }
    }
}
