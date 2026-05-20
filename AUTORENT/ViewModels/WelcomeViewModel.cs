using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class OnboardingItem
    {
        public string ImageSource { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class WelcomeViewModel : BaseViewModel
    {
        private ObservableCollection<OnboardingItem> _items;
        private int _currentIndex;
        private System.Threading.Timer? _autoPlayTimer;

        public WelcomeViewModel()
        {
            _items = new ObservableCollection<OnboardingItem>
            {
                new OnboardingItem
                {
                    ImageSource = "onboarding_1.png",
                    Title = "Encuentra. Reserva. Estaciona.",
                    Description = "Localiza lugares disponibles cerca de ti en tiempo real."
                },
                new OnboardingItem
                {
                    ImageSource = "onboarding_2.png",
                    Title = "Renta tu auto ideal",
                    Description = "Miles de vehículos disponibles con entrega a domicilio."
                },
                new OnboardingItem
                {
                    ImageSource = "onboarding_3.png",
                    Title = "Reserva en segundos",
                    Description = "Proceso rápido y seguro desde tu celular."
                },
                new OnboardingItem
                {
                    ImageSource = "onboarding_4.png",
                    Title = "Gana dinero con tu auto",
                    Description = "Renta tu vehículo y genera ingresos extra de forma segura."
                }
            };

            Title = "Bienvenido";
            
            LoginCommand = new AsyncRelayCommand(NavigateToLoginAsync);
            RegisterCommand = new AsyncRelayCommand(NavigateToRegisterAsync);

            StartAutoPlay();
        }

        public ObservableCollection<OnboardingItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (SetProperty(ref _currentIndex, value))
                {
                    OnPropertyChanged(nameof(CurrentTitle));
                    OnPropertyChanged(nameof(CurrentDescription));
                    OnPropertyChanged(nameof(CurrentImageSource));
                }
            }
        }

        public string CurrentTitle => Items[CurrentIndex].Title;
        public string CurrentDescription => Items[CurrentIndex].Description;
        public string CurrentImageSource => Items[CurrentIndex].ImageSource;
        public int ItemsCount => Items.Count;

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public void StartAutoPlay()
        {
            _autoPlayTimer = new System.Threading.Timer((state) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CurrentIndex = (CurrentIndex + 1) % Items.Count;
                });
            }, null, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4));
        }

        public void StopAutoPlay()
        {
            _autoPlayTimer?.Dispose();
        }

        private async Task NavigateToLoginAsync()
        {
            _autoPlayTimer?.Dispose();
            Preferences.Set("HasSeenOnboarding", true);
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task NavigateToRegisterAsync()
        {
            _autoPlayTimer?.Dispose();
            Preferences.Set("HasSeenOnboarding", true);
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
