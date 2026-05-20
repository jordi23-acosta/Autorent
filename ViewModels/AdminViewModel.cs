using AutoRent.Models;
using AutoRent.Services;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AutoRent.ViewModels;

public class AdminViewModel : BaseViewModel
{
    private readonly SupabaseService _supabase;

    public ObservableCollection<Auto> Autos { get; } = new ObservableCollection<Auto>();

    private Auto _selectedAuto;
    public Auto SelectedAuto { get => _selectedAuto; set { SetProperty(ref _selectedAuto, value); if (value != null) EditingAuto = new Auto { Id = value.Id, Marca = value.Marca, Modelo = value.Modelo, Ano = value.Ano, Tipo = value.Tipo, PrecioHora = value.PrecioHora, PrecioDia = value.PrecioDia, Disponible = value.Disponible, ImagenUrl = value.ImagenUrl }; } }

    private Auto _editingAuto = new Auto();
    public Auto EditingAuto { get => _editingAuto; set => SetProperty(ref _editingAuto, value); }

    public ICommand NewCarCommand { get; }
    public ICommand EditCarCommand { get; }
    public ICommand SaveCarCommand { get; }
    public ICommand DeleteCarCommand { get; }
    public ICommand PickImageCommand { get; }

    public AdminViewModel()
    {
        _supabase = App.Services.GetService(typeof(SupabaseService)) as SupabaseService;
        NewCarCommand = new Command(() => { EditingAuto = new Auto { Disponible = true }; Shell.Current.GoToAsync("/admin/edit"); });
        EditCarCommand = new Command<object>(async (obj) => { SelectedAuto = obj as Auto; await Shell.Current.GoToAsync("/admin/edit"); });
        SaveCarCommand = new Command(async () => await SaveAsync());
        DeleteCarCommand = new Command(async () => await DeleteAsync());
        PickImageCommand = new Command(async () => await PickImageAsync());

        _ = LoadAutosAsync();
    }

    public async Task LoadAutosAsync()
    {
        var list = await _supabase.GetAutosAsync();
        Autos.Clear();
        foreach (var a in list) Autos.Add(a);
    }

    private async Task SaveAsync()
    {
        if (EditingAuto.Id == 0)
        {
            var resp = await _supabase.CreateAutoAsync(EditingAuto);
            if (resp.IsSuccessStatusCode) await Application.Current.MainPage.DisplayAlert("OK", "Auto creado", "OK");
        }
        else
        {
            var resp = await _supabase.UpdateAutoAsync(EditingAuto.Id, EditingAuto);
            if (resp.IsSuccessStatusCode) await Application.Current.MainPage.DisplayAlert("OK", "Auto actualizado", "OK");
        }
        await LoadAutosAsync();
        await Shell.Current.GoToAsync("/admin");
    }

    private async Task DeleteAsync()
    {
        if (EditingAuto == null || EditingAuto.Id == 0) return;
        var resp = await _supabase.DeleteAutoAsync(EditingAuto.Id);
        if (resp.IsSuccessStatusCode) await Application.Current.MainPage.DisplayAlert("OK", "Auto eliminado", "OK");
        await LoadAutosAsync();
        await Shell.Current.GoToAsync("/admin");
    }

    private async Task PickImageAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Seleccionar imagen" });
            if (result == null) return;
            using var stream = await result.OpenReadAsync();
            var filename = System.IO.Path.GetFileName(result.FileName);
            var path = $"cars/{Guid.NewGuid()}_{filename}";
            var url = await _supabase.UploadFileAsync("public", path, stream, result.ContentType ?? "image/jpeg");
            if (url != null) EditingAuto.ImagenUrl = url;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
