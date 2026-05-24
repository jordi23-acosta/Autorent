using AUTORENT.Models;
using AUTORENT.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class OwnerEarningsViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        private decimal _totalEarnings;
        private decimal _currentMonthEarnings;
        private decimal _pendingEarnings;
        private int _pendingRentalsCount;
        private int _currentMonthRentals;
        private string _trendText = string.Empty;
        private double _trendPercentage;
        private bool _trendUp = true;
        private bool _hasEarnings;
        private bool _hasMonthlyHistory;
        private bool _hasTopVehicles;

        private ObservableCollection<MonthlyEarning> _monthlyHistory;
        private ObservableCollection<TopVehicle> _topVehicles;

        public OwnerEarningsViewModel()
        {
            _authService = AuthService.Instance;
            _monthlyHistory = new ObservableCollection<MonthlyEarning>();
            _topVehicles = new ObservableCollection<TopVehicle>();

            Title = "Mis Ganancias";

            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
            WithdrawCommand = new AsyncRelayCommand(WithdrawAsync);
        }

        // Properties
        public decimal TotalEarnings
        {
            get => _totalEarnings;
            set
            {
                if (SetProperty(ref _totalEarnings, value))
                {
                    OnPropertyChanged(nameof(TotalEarningsFormatted));
                    OnPropertyChanged(nameof(HasEarnings));
                }
            }
        }

        public decimal CurrentMonthEarnings
        {
            get => _currentMonthEarnings;
            set
            {
                if (SetProperty(ref _currentMonthEarnings, value))
                    OnPropertyChanged(nameof(CurrentMonthEarningsFormatted));
            }
        }

        public decimal PendingEarnings
        {
            get => _pendingEarnings;
            set
            {
                if (SetProperty(ref _pendingEarnings, value))
                    OnPropertyChanged(nameof(PendingEarningsFormatted));
            }
        }

        public int PendingRentalsCount
        {
            get => _pendingRentalsCount;
            set
            {
                if (SetProperty(ref _pendingRentalsCount, value))
                    OnPropertyChanged(nameof(PendingRentalsText));
            }
        }

        public int CurrentMonthRentals
        {
            get => _currentMonthRentals;
            set => SetProperty(ref _currentMonthRentals, value);
        }

        public string TrendText
        {
            get => _trendText;
            set => SetProperty(ref _trendText, value);
        }

        public double TrendPercentage
        {
            get => _trendPercentage;
            set => SetProperty(ref _trendPercentage, value);
        }

        public bool TrendUp
        {
            get => _trendUp;
            set => SetProperty(ref _trendUp, value);
        }

        public bool HasEarnings
        {
            get => _hasEarnings;
            set => SetProperty(ref _hasEarnings, value);
        }

        public bool HasMonthlyHistory
        {
            get => _hasMonthlyHistory;
            set => SetProperty(ref _hasMonthlyHistory, value);
        }

        public bool HasTopVehicles
        {
            get => _hasTopVehicles;
            set => SetProperty(ref _hasTopVehicles, value);
        }

        public ObservableCollection<MonthlyEarning> MonthlyHistory
        {
            get => _monthlyHistory;
            set => SetProperty(ref _monthlyHistory, value);
        }

        public ObservableCollection<TopVehicle> TopVehicles
        {
            get => _topVehicles;
            set => SetProperty(ref _topVehicles, value);
        }

        // Computed properties
        public string TotalEarningsFormatted => $"{TotalEarnings:N0}";
        public string CurrentMonthEarningsFormatted => $"${CurrentMonthEarnings:N0}";
        public string PendingEarningsFormatted => $"${PendingEarnings:N0}";
        public string PendingRentalsText => PendingRentalsCount == 1 
            ? "1 renta" 
            : $"{PendingRentalsCount} rentas";

        public ICommand RefreshCommand { get; }
        public ICommand WithdrawCommand { get; }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var client = _authService.GetClient();
                if (client == null || _authService.CurrentUser == null) return;

                var userId = _authService.CurrentUser.Id;

                // Cargar todas las rentas del propietario
                var rentalsResponse = await client
                    .From<Rental>()
                    .Where(x => x.OwnerId == userId)
                    .Get();

                var allRentals = rentalsResponse?.Models ?? new List<Rental>();

                // Calcular ganancias totales (rentas completadas)
                var completedRentals = allRentals
                    .Where(r => r.StatusString == "completada")
                    .ToList();
                TotalEarnings = completedRentals.Sum(r => r.TotalPrice);

                // Ganancias del mes actual
                var now = DateTime.Now;
                var currentMonthRentals = completedRentals
                    .Where(r => r.EndDate.Year == now.Year && r.EndDate.Month == now.Month)
                    .ToList();
                CurrentMonthEarnings = currentMonthRentals.Sum(r => r.TotalPrice);
                CurrentMonthRentals = currentMonthRentals.Count;

                // Ganancias del mes anterior (para calcular tendencia)
                var lastMonth = now.AddMonths(-1);
                var lastMonthEarnings = completedRentals
                    .Where(r => r.EndDate.Year == lastMonth.Year && r.EndDate.Month == lastMonth.Month)
                    .Sum(r => r.TotalPrice);

                // Calcular tendencia
                if (lastMonthEarnings > 0)
                {
                    var diff = ((CurrentMonthEarnings - lastMonthEarnings) / lastMonthEarnings) * 100;
                    TrendPercentage = (double)diff;
                    TrendUp = diff >= 0;
                    TrendText = TrendUp 
                        ? $"↗ +{Math.Abs(TrendPercentage):F0}% vs mes anterior" 
                        : $"↘ -{Math.Abs(TrendPercentage):F0}% vs mes anterior";
                }
                else if (CurrentMonthEarnings > 0)
                {
                    TrendText = "🎉 ¡Primer mes con ganancias!";
                    TrendUp = true;
                }
                else
                {
                    TrendText = "Sin actividad este mes";
                    TrendUp = true;
                }

                // Pendientes (rentas activas o confirmadas)
                var pendingRentals = allRentals
                    .Where(r => r.StatusString == "activa" || 
                                r.StatusString == "confirmada" || 
                                r.StatusString == "pendiente")
                    .ToList();
                PendingEarnings = pendingRentals.Sum(r => r.TotalPrice);
                PendingRentalsCount = pendingRentals.Count;

                HasEarnings = TotalEarnings > 0 || CurrentMonthEarnings > 0;

                // Histórico mensual (últimos 6 meses)
                MonthlyHistory.Clear();
                for (int i = 0; i < 6; i++)
                {
                    var month = now.AddMonths(-i);
                    var monthRentals = completedRentals
                        .Where(r => r.EndDate.Year == month.Year && r.EndDate.Month == month.Month)
                        .ToList();
                    
                    if (monthRentals.Count > 0 || i < 3)
                    {
                        MonthlyHistory.Add(new MonthlyEarning
                        {
                            Month = month.ToString("MMMM yyyy"),
                            RentalsCount = monthRentals.Count,
                            Earnings = monthRentals.Sum(r => r.TotalPrice)
                        });
                    }
                }

                // Top vehículos (ranking)
                if (completedRentals.Count > 0)
                {
                    var vehicleIds = completedRentals.Select(r => r.VehicleId).Distinct().ToList();
                    var vehiclesResponse = await client
                        .From<Vehicle>()
                        .Filter("id", Supabase.Postgrest.Constants.Operator.In, vehicleIds)
                        .Get();
                    
                    var vehicles = vehiclesResponse?.Models?.ToDictionary(v => v.Id) 
                        ?? new Dictionary<string, Vehicle>();

                    var topVehiclesData = completedRentals
                        .GroupBy(r => r.VehicleId)
                        .Select(g => new
                        {
                            VehicleId = g.Key,
                            Count = g.Count(),
                            Total = g.Sum(r => r.TotalPrice)
                        })
                        .OrderByDescending(x => x.Total)
                        .Take(5)
                        .ToList();

                    TopVehicles.Clear();
                    int rank = 1;
                    foreach (var data in topVehiclesData)
                    {
                        if (vehicles.TryGetValue(data.VehicleId, out var vehicle))
                        {
                            TopVehicles.Add(new TopVehicle
                            {
                                Rank = rank++,
                                VehicleName = vehicle.DisplayName,
                                RentalsCount = data.Count,
                                PricePerDay = vehicle.PricePerDay,
                                TotalEarnings = data.Total,
                                Brand = vehicle.Brand
                            });
                        }
                    }
                }

                HasMonthlyHistory = MonthlyHistory.Count > 0;
                HasTopVehicles = TopVehicles.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EARNINGS] Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task WithdrawAsync()
        {
            if (TotalEarnings <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Sin fondos",
                    "Aún no tienes ganancias disponibles para retirar",
                    "OK");
                return;
            }

            await Application.Current!.MainPage!.DisplayAlert(
                "💸 Retiro de Fondos",
                $"Tu solicitud de retiro por ${TotalEarnings:N0} será procesada en 2-3 días hábiles.",
                "OK");
        }
    }

    public class MonthlyEarning
    {
        public string Month { get; set; } = string.Empty;
        public int RentalsCount { get; set; }
        public decimal Earnings { get; set; }

        public string EarningsFormatted => $"${Earnings:N0}";
        public string RentalsText => RentalsCount == 1 
            ? "1 renta completada" 
            : $"{RentalsCount} rentas completadas";
        public bool HasEarnings => Earnings > 0;
        public Color EarningsColor => Earnings > 0 
            ? Color.FromArgb("#43A047") 
            : Color.FromArgb("#BDBDBD");
    }

    public class TopVehicle
    {
        public int Rank { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int RentalsCount { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal TotalEarnings { get; set; }

        public string SubtitleText => $"{RentalsCount} rentas • ${PricePerDay:F0}/día";
        public string TotalFormatted => $"${TotalEarnings:N0}";
        public string Emoji => Brand.ToLower() switch
        {
            var b when b.Contains("toyota") => "🚙",
            var b when b.Contains("honda") => "🚐",
            var b when b.Contains("bmw") => "🏎️",
            var b when b.Contains("mercedes") => "🚗",
            _ => "🚗"
        };
        public Color RankColor => Rank switch
        {
            1 => Color.FromArgb("#FFD700"),
            2 => Color.FromArgb("#C0C0C0"),
            3 => Color.FromArgb("#CD7F32"),
            _ => Color.FromArgb("#9E9E9E")
        };
    }
}
