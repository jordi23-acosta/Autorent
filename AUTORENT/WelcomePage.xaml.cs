using AUTORENT.ViewModels;

namespace AUTORENT;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel _viewModel;

    public WelcomePage()
    {
        InitializeComponent();
        _viewModel = new WelcomeViewModel();
        BindingContext = _viewModel;

        BuildDots();
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _viewModel.StartAutoPlay();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.CurrentIndex))
        {
            UpdateDots(_viewModel.CurrentIndex);
            BackgroundImage.Source = _viewModel.CurrentImageSource;
        }
    }

    private void BuildDots()
    {
        DotsContainer.Children.Clear();
        for (int i = 0; i < _viewModel.ItemsCount; i++)
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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopAutoPlay();
    }
}
