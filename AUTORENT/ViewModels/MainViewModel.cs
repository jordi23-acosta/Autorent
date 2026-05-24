using AUTORENT.Models;
using AUTORENT.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private ObservableCollection<Vehicle> _availableVehicles;
        private ObservableCollection<Vehicle> _allVehicles;
        private string _searchQuery = string.Empty;
        private string _selectedCategory = "Todos";
        private bool _hasNoResults;
        private string _userName = string.Empty;
        private int _totalVehicles;

        public MainViewModel()
        {
            _authService = AuthService.Instance;
            _availableVehicles = new ObservableCollection<Vehicle>();
            _allVehicles = new ObservableCollection<Vehicle>();

            Title = "Inicio";

            SearchCommand = new AsyncRelayCommand(SearchAsync);
            RefreshCommand = new AsyncRelayCommand(LoadAvailableVehiclesAsync);
            FilterByCategoryCommand = new RelayCommand<string>(FilterByCategory);
            ClearSearchCommand = new RelayCommand(ClearSearch);

            // Obtener nombre del usuario
            if (_authService.CurrentUser != null)
            {
                UserName = _authService.CurrentUser.Name;
            }

            _ = LoadAvailableVehiclesAsync();
        }

        public ObservableCollection<Vehicle> AvailableVehicles
        {
            get => _availableVehicles;
            set => SetProperty(ref _availableVehicles, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public bool HasNoResults
        {
            get => _hasNoResults;
            set => SetProperty(ref _hasNoResults, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public int TotalVehicles
        {
            get => _totalVehicles;
            set => SetProperty(ref _totalVehicles, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand FilterByCategoryCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public async Task LoadAvailableVehiclesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null) return;

                var vehiclesResponse = await client
                    .From<Vehicle>()
                    .Where(x => x.IsAvailable == true)
                    .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(50)
                    .Get();

                _allVehicles.Clear();
                AvailableVehicles.Clear();

                if (vehiclesResponse?.Models != null)
                {
                    foreach (var vehicle in vehiclesResponse.Models)
                    {
                        _allVehicles.Add(vehicle);
                        AvailableVehicles.Add(vehicle);
                    }
                }

                TotalVehicles = AvailableVehicles.Count;
                HasNoResults = TotalVehicles == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando vehículos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allVehicles.AsEnumerable();

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                var query = _searchQuery.ToLower();
                filtered = filtered.Where(v =>
                    v.Brand.ToLower().Contains(query) ||
                    v.Model.ToLower().Contains(query) ||
                    (v.Location?.ToLower().Contains(query) ?? false));
            }

            // Filtro por categoría
            if (_selectedCategory != "Todos")
            {
                filtered = _selectedCategory switch
                {
                    "Económicos" => filtered.Where(v => v.PricePerDay < 800),
                    "Premium" => filtered.Where(v => v.PricePerDay >= 800 && v.PricePerDay < 1500),
                    "Lujo" => filtered.Where(v => v.PricePerDay >= 1500),
                    _ => filtered
                };
            }

            AvailableVehicles.Clear();
            foreach (var vehicle in filtered)
            {
                AvailableVehicles.Add(vehicle);
            }

            TotalVehicles = AvailableVehicles.Count;
            HasNoResults = TotalVehicles == 0;
        }

        private void FilterByCategory(string? category)
        {
            if (string.IsNullOrEmpty(category)) return;
            SelectedCategory = category;
            ApplyFilters();
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private async Task SearchAsync()
        {
            ApplyFilters();
            await Task.CompletedTask;
        }

        public string GetVehicleEmoji(string brand)
        {
            return brand.ToLower() switch
            {
                var b when b.Contains("toyota") => "🚙",
                var b when b.Contains("honda") => "🚐",
                var b when b.Contains("bmw") => "🏎️",
                var b when b.Contains("mercedes") => "🚗",
                var b when b.Contains("ford") => "🚙",
                var b when b.Contains("chevrolet") => "🚙",
                var b when b.Contains("nissan") => "🚗",
                var b when b.Contains("mazda") => "🚗",
                var b when b.Contains("audi") => "🏎️",
                var b when b.Contains("volkswagen") => "🚗",
                var b when b.Contains("hyundai") => "🚙",
                var b when b.Contains("kia") => "🚙",
                _ => "🚗"
            };
        }

        public Color GetCategoryColor(decimal price)
        {
            if (price < 800) return Color.FromArgb("#4CAF50"); // Verde - Económico
            if (price < 1500) return Color.FromArgb("#FF9800"); // Naranja - Premium
            return Color.FromArgb("#9C27B0"); // Morado - Lujo
        }

        public string GetCategoryLabel(decimal price)
        {
            if (price < 800) return "Económico";
            if (price < 1500) return "Premium";
            return "Lujo";
        }
    }
}
