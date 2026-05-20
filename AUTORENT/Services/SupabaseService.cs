using Supabase;

namespace AUTORENT.Services;

public static class SupabaseService
{
    private const string SUPABASE_URL = "https://whxqhjzodvyvqhmhbwaw.supabase.co";
    private const string SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndoeHFoanpvZHZ5dnFobWhid2F3Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzkyOTgzOTEsImV4cCI6MjA5NDg3NDM5MX0.f5UGJ8Mm0ZPsll877tqHDZjlYllhBzV85ou9UwxHCsc";

    private static Supabase.Client? _client;

    public static Supabase.Client Client
    {
        get
        {
            if (_client == null)
            {
                _client = new Supabase.Client(SUPABASE_URL, SUPABASE_ANON_KEY);
            }
            return _client;
        }
    }

    public static async Task InitializeAsync()
    {
        await Client.InitializeAsync();
    }
}
