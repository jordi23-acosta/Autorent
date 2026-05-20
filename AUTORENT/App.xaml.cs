using AUTORENT.Services;

namespace AUTORENT
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            // Inicializar tema antes de crear ventanas
            ThemeService.Instance.LoadSavedTheme();
        }

        private async Task InitializeSupabaseAsync()
        {
            // Credenciales de Supabase
            const string SUPABASE_URL = "https://whxqhjzodvyvqhmhbwaw.supabase.co";
            const string SUPABASE_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndoeHFoanpvZHZ5dnFobWhid2F3Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzkyOTgzOTEsImV4cCI6MjA5NDg3NDM5MX0.f5UGJ8Mm0ZPsll877tqHDZjlYllhBzV85ou9UwxHCsc";

            try
            {
                System.Diagnostics.Debug.WriteLine("[APP] 🚀 Inicializando Supabase...");
                await AuthService.Instance.InitializeAsync(SUPABASE_URL, SUPABASE_KEY);
                System.Diagnostics.Debug.WriteLine("[APP] ✅ Supabase inicializado correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[APP] ❌ Error inicializando Supabase: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[APP] Stack: {ex.StackTrace}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            System.Diagnostics.Debug.WriteLine("[APP] 🪟 Creando ventana...");
            
            // Crear página de carga
            var loadingPage = new LoadingPage();
            var window = new Window(loadingPage);

            // Inicializar de forma asíncrona sin bloquear
            Task.Run(async () =>
            {
                try
                {
                    await InitializeSupabaseAsync();

                    var authService = AuthService.Instance;
                    
                    System.Diagnostics.Debug.WriteLine($"[APP] 🔍 Verificando estado de autenticación...");
                    System.Diagnostics.Debug.WriteLine($"[APP] IsLoggedIn: {authService.IsLoggedIn}");
                    System.Diagnostics.Debug.WriteLine($"[APP] CurrentUser: {authService.CurrentUser?.Email ?? "null"}");

                    // Cambiar a la página correcta en el hilo principal
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (authService.IsLoggedIn && authService.CurrentUser != null)
                        {
                            System.Diagnostics.Debug.WriteLine("[APP] ✅ Usuario autenticado - mostrando AppShell");
                            System.Diagnostics.Debug.WriteLine($"[APP] 👤 Usuario: {authService.CurrentUser.Name} ({authService.CurrentUser.Email})");
                            window.Page = new AppShell();
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[APP] ℹ️ Usuario no autenticado - mostrando LoginPage");
                            window.Page = new NavigationPage(new LoginPage());
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[APP] ❌ Error en inicialización: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[APP] Stack: {ex.StackTrace}");
                    
                    // En caso de error, mostrar login
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("[APP] 🔄 Mostrando LoginPage por error");
                        window.Page = new NavigationPage(new LoginPage());
                    });
                }
            });

            return window;
        }
    }
}