using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AUTORENT.ViewModels
{
    public class OnboardingItem
    {
        public string ImageSource { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Emoji { get; set; } = "";
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
                    Emoji = "🚗",
                    Title = "Encuentra el auto perfecto",
                    Description = "Explora cientos de vehículos disponibles cerca de ti."
                },
                new OnboardingItem
                {
                    ImageSource = "onboarding_2.png",
                    Emoji = "🔍",
                    Title = "Renta en segundos",
                    Description = "Proceso rápido y seguro desde tu celular."
                },
                new OnboardingItem
                {
                    ImageSource = "onboarding_3.png",
                    Emoji = "📅",
                    Title = "Reserva con confianza",
                    Description = "Confirmación inmediata y entrega flexible."
                },
                new OnboardingItem
                {
                    ImageSource = "onboarding_4.png",
                    Emoji = "💰",
                    Title = "Gana dinero con tu auto",
                    Description = "Publica tu vehículo y genera ingresos extra."
                }
            };

            Title = "Bienvenido";
            
            LoginCommand = new AsyncRelayCommand(NavigateToLoginAsync);
            RegisterCommand = new AsyncRelayCommand(NavigateToRegisterAsync);
            SkipCommand = new AsyncRelayCommand(NavigateToLoginAsync);
            NextCommand = new RelayCommand(NextSlide);
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
                    OnPropertyChanged(nameof(CurrentEmoji));
                    OnPropertyChanged(nameof(IsLastSlide));
                    OnPropertyChanged(nameof(SlideNumber));
                }
            }
        }

        public string CurrentTitle => Items[CurrentIndex].Title;
        public string CurrentDescription => Items[CurrentIndex].Description;
        public string CurrentImageSource => Items[CurrentIndex].ImageSource;
        public string CurrentEmoji => Items[CurrentIndex].Emoji;
        public int ItemsCount => Items.Count;
        public bool IsLastSlide => CurrentIndex == Items.Count - 1;
        public string SlideNumber => $"{CurrentIndex + 1} / {Items.Count}";

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand SkipCommand { get; }
        public ICommand NextCommand { get; }

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
            _autoPlayTimer = null;
        }

        private void NextSlide()
        {
            if (CurrentIndex < Items.Count - 1)
            {
                CurrentIndex++;
            }
            else
            {
                _ = NavigateToRegisterAsync();
            }
        }

        private async Task NavigateToLoginAsync()
        {
            StopAutoPlay();
            Preferences.Set("HasSeenOnboarding", true);

            try
            {
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando a Login: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task NavigateToRegisterAsync()
        {
            StopAutoPlay();
            Preferences.Set("HasSeenOnboarding", true);

            try
            {
                if (Application.Current != null)
                {
                    var navPage = new NavigationPage(new LoginPage());
                    Application.Current.MainPage = navPage;
                    await navPage.PushAsync(new RegisterPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando a Register: {ex.Message}");
            }
        }
    }
}
