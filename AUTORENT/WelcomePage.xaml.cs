namespace AUTORENT;

public class OnboardingItem
{
    public string ImageSource { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

public partial class WelcomePage : ContentPage
{
    private List<OnboardingItem> _items;
    private System.Threading.Timer? _autoPlayTimer;
    private int _currentIndex = 0;

    public WelcomePage()
    {
        InitializeComponent();

        _items = new List<OnboardingItem>
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

        BuildDots();
        ShowSlide(0);
        StartAutoPlay();
    }

    private void BuildDots()
    {
        DotsContainer.Children.Clear();
        for (int i = 0; i < _items.Count; i++)
        {
            DotsContainer.Children.Add(new BoxView
            {
                WidthRequest = 8,
                HeightRequest = 8,
                CornerRadius = 4,
                Color = i == 0 ? Color.FromArgb("#00E5CC") : Color.FromArgb("#60FFFFFF"),
                VerticalOptions = LayoutOptions.Center
            });
        }
    }

    private void UpdateDots(int activeIndex)
    {
        for (int i = 0; i < DotsContainer.Children.Count; i++)
        {
            if (DotsContainer.Children[i] is BoxView dot)
                dot.Color = i == activeIndex
                    ? Color.FromArgb("#00E5CC")
                    : Color.FromArgb("#60FFFFFF");
        }
    }

    // Sin animación — cambio directo, sin negro
    private void ShowSlide(int index)
    {
        var item = _items[index];
        BackgroundImage.Source = item.ImageSource;
        TitleLabel.Text = item.Title;
        DescriptionLabel.Text = item.Description;
        UpdateDots(index);
    }

    private void StartAutoPlay()
    {
        _autoPlayTimer = new System.Threading.Timer((state) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _currentIndex = (_currentIndex + 1) % _items.Count;
                ShowSlide(_currentIndex);
            });
        }, null, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        _autoPlayTimer?.Dispose();
        Preferences.Set("HasSeenOnboarding", true);
        await Navigation.PushAsync(new LoginPage());
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        _autoPlayTimer?.Dispose();
        Preferences.Set("HasSeenOnboarding", true);
        await Navigation.PushAsync(new LoginPage());
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _autoPlayTimer?.Dispose();
    }
}
