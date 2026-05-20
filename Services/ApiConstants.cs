namespace AutoRent.Services;

public static class ApiConstants
{
    // Rellena con tus datos de Supabase
    public const string SUPABASE_URL = "https://your-project.supabase.co";
    public const string SUPABASE_ANON_KEY = "your-anon-key";

    public static string RestUrl => $"{SUPABASE_URL}/rest/v1";
    public static string AuthUrl => $"{SUPABASE_URL}/auth/v1";
}
