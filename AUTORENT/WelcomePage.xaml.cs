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
        UpdateBackgroundImage();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartAutoPlay();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.CurrentIndex))
        {
            UpdateDots(_viewModel.CurrentIndex);
            UpdateBackgroundImage();
        }
    }

    private void UpdateBackgroundImage()
    {
        try
        {
            BackgroundImage.Source = _viewModel.CurrentImageSource;
        }
        catch { }
    }

    private void BuildDots()
    {
        DotsContainer.Children.Clear();
        for (int i = 0; i < _viewModel.ItemsCount; i++)
        {
            var isActive = i == _viewModel.CurrentIndex;
            var dot = new Border
            {
                WidthRequest = isActive ? 24 : 8,
                HeightRequest = 8,
                StrokeThickness = 0,
                BackgroundColor = isActive 
                    ? Color.FromArgb("#00E5CC") 
                    : Color.FromArgb("#60FFFFFF"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle 
                { 
                    CornerRadius = 4 
                },
                VerticalOptions = LayoutOptions.Center
            };
            DotsContainer.Children.Add(dot);
        }
    }

    private void UpdateDots(int activeIndex)
    {
        for (int i = 0; i < DotsContainer.Children.Count; i++)
        {
            if (DotsContainer.Children[i] is Border dot)
            {
                bool isActive = i == activeIndex;
                dot.WidthRequest = isActive ? 24 : 8;
                dot.BackgroundColor = isActive
                    ? Color.FromArgb("#00E5CC")
                    : Color.FromArgb("#60FFFFFF");
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopAutoPlay();
    }
}
