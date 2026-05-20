using AutoRent.Models;
using AutoRent.Services;
using System.Collections.ObjectModel;

namespace AutoRent.ViewModels;

public class CarsViewModel : BaseViewModel
{
    private readonly SupabaseService _supabase;
    public ObservableCollection<Auto> Autos { get; } = new ObservableCollection<Auto>();

    public CarsViewModel()
    {
        // En ejecución real, SupabaseService debería inyectarse vía constructor
        try
        {
            _supabase = App.Current.Services.GetService(typeof(SupabaseService)) as SupabaseService ?? throw new Exception();
        }
        catch
        {
            _supabase = null;
        }

        // Cargar datos
        _ = LoadAutosAsync();
    }

    public async Task LoadAutosAsync()
    {
        if (_supabase == null) return;
        var list = await _supabase.GetAutosAsync();
        Autos.Clear();
        foreach (var a in list) Autos.Add(a);
    }
}
