using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoRent.Models;
using Microsoft.Maui.Storage;

namespace AutoRent.Services;

public class SupabaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private const string ACCESS_TOKEN_KEY = "supabase_access_token";
    private const string REFRESH_TOKEN_KEY = "supabase_refresh_token";
    private const string USER_JSON_KEY = "supabase_user";

    public SupabaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient(bool includeAuth = true)
    {
        var client = _httpClientFactory.CreateClient("Supabase");
        client.BaseAddress = new Uri(ApiConstants.SUPABASE_URL);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("apikey", ApiConstants.SUPABASE_ANON_KEY);
        if (includeAuth)
        {
            var token = GetAccessTokenAsync().GetAwaiter().GetResult();
            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            else
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiConstants.SUPABASE_ANON_KEY}");
        }
        else
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiConstants.SUPABASE_ANON_KEY}");
        }
        return client;
    }

    // Guardar tokens en SecureStorage
    private async Task SaveSessionAsync(JsonElement authJson)
    {
        if (authJson.TryGetProperty("access_token", out var at))
            await SecureStorage.SetAsync(ACCESS_TOKEN_KEY, at.GetString() ?? string.Empty);
        if (authJson.TryGetProperty("refresh_token", out var rt))
            await SecureStorage.SetAsync(REFRESH_TOKEN_KEY, rt.GetString() ?? string.Empty);
        if (authJson.TryGetProperty("user", out var user))
            await SecureStorage.SetAsync(USER_JSON_KEY, user.GetRawText());
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(ACCESS_TOKEN_KEY) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(REFRESH_TOKEN_KEY) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            SecureStorage.Remove(ACCESS_TOKEN_KEY);
            SecureStorage.Remove(REFRESH_TOKEN_KEY);
            SecureStorage.Remove(USER_JSON_KEY);
        }
        catch { }
    }

    // Registro (signup) y guardado de sesión cuando aplica
    public async Task<HttpResponseMessage> RegisterAsync(string email, string password)
    {
        var client = CreateClient(includeAuth: false);
        var payload = new { email, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var resp = await client.PostAsync($"{ApiConstants.AuthUrl}/signup", content);
        if (resp.IsSuccessStatusCode)
        {
            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            // Supabase signup returns user field but not necessarily tokens; try to save user
            if (root.TryGetProperty("access_token", out _) || root.TryGetProperty("user", out _))
            {
                await SaveSessionAsync(root);
            }
        }
        return resp;
    }

    // Login (almacena tokens en SecureStorage)
    public async Task<HttpResponseMessage> LoginAsync(string email, string password)
    {
        var client = CreateClient(includeAuth: false);
        var payload = new { email, password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var resp = await client.PostAsync($"{ApiConstants.AuthUrl}/token?grant_type=password", content);
        if (resp.IsSuccessStatusCode)
        {
            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            await SaveSessionAsync(root);
        }
        return resp;
    }

    // Refresh token
    public async Task<bool> RefreshTokenAsync()
    {
        var refresh = await GetRefreshTokenAsync();
        if (string.IsNullOrWhiteSpace(refresh)) return false;
        var client = CreateClient(includeAuth: false);
        var payload = new { refresh_token = refresh };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var resp = await client.PostAsync($"{ApiConstants.AuthUrl}/token?grant_type=refresh_token", content);
        if (!resp.IsSuccessStatusCode) return false;
        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        await SaveSessionAsync(root);
        return true;
    }

    // Traer autos
    public async Task<List<Auto>> GetAutosAsync()
    {
        var client = CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Get, $"{ApiConstants.RestUrl}/autos?select=*");
        var resp = await client.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Auto>>(json, _jsonOptions) ?? new List<Auto>();
    }

    // Crear/Actualizar/Eliminar autos (CRUD simple)
    public async Task<HttpResponseMessage> CreateAutoAsync(Auto auto)
    {
        var client = CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(auto), Encoding.UTF8, "application/json");
        return await client.PostAsync($"{ApiConstants.RestUrl}/autos", content);
    }

    public async Task<HttpResponseMessage> UpdateAutoAsync(int id, Auto auto)
    {
        var client = CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(auto), Encoding.UTF8, "application/json");
        return await client.PatchAsync($"{ApiConstants.RestUrl}/autos?id=eq.{id}", content);
    }

    public async Task<HttpResponseMessage> DeleteAutoAsync(int id)
    {
        var client = CreateClient();
        return await client.DeleteAsync($"{ApiConstants.RestUrl}/autos?id=eq.{id}");
    }

    // Renta
    public async Task<HttpResponseMessage> CreateRentaAsync(Renta renta)
    {
        var client = CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(renta), Encoding.UTF8, "application/json");
        return await client.PostAsync($"{ApiConstants.RestUrl}/rentas", content);
    }

    // Traer rentas activas
    public async Task<List<Renta>> GetRentasActivasAsync()
    {
        var client = CreateClient();
        var resp = await client.GetAsync($"{ApiConstants.RestUrl}/rentas?estado=eq.activa");
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Renta>>(json, _jsonOptions) ?? new List<Renta>();
    }

    // Subir archivo a Supabase Storage (usa PUT). Devuelve la URL pública si todo OK.
    public async Task<string?> UploadFileAsync(string bucket, string path, Stream fileStream, string contentType, bool makePublic = true)
    {
        // PUT /storage/v1/object/{bucket}/{path}
        var client = CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Put, $"{ApiConstants.SUPABASE_URL}/storage/v1/object/{bucket}/{path}")
        {
            Content = new StreamContent(fileStream)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        // opcional: forzar creación
        request.Headers.Add("x-upsert", "true");

        var resp = await client.SendAsync(request);
        if (!resp.IsSuccessStatusCode) return null;

        if (makePublic)
        {
            // URL pública estándar
            return $"{ApiConstants.SUPABASE_URL}/storage/v1/object/public/{bucket}/{Uri.EscapeDataString(path)}";
        }
        return null;
    }
}
