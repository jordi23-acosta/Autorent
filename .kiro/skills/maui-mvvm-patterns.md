# .NET MAUI MVVM Patterns & Architecture

## 🏗️ MVVM Architecture Overview

### The Three Layers

```
┌─────────────────────────────────────┐
│           VIEW (XAML)               │
│  - User Interface                   │
│  - Data Bindings                    │
│  - No Business Logic                │
└──────────────┬──────────────────────┘
               │ Bindings
┌──────────────▼──────────────────────┐
│         VIEW MODEL                  │
│  - Presentation Logic               │
│  - Commands                         │
│  - Properties                       │
│  - INotifyPropertyChanged           │
└──────────────┬──────────────────────┘
               │ Uses
┌──────────────▼──────────────────────┐
│           MODEL                     │
│  - Business Logic                   │
│  - Data Access                      │
│  - Validation                       │
└─────────────────────────────────────┘
```

## 📝 BaseViewModel Pattern

### Complete BaseViewModel
```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private string _title;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

## ⚡ Command Patterns

### RelayCommand Implementation
```csharp
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object parameter) => _execute();

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

    public void Execute(object parameter) => _execute((T)parameter);

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

### AsyncRelayCommand Implementation
```csharp
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        if (_isExecuting)
            return;

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

## 🎯 ViewModel Examples

### Simple ViewModel
```csharp
public class LoginViewModel : BaseViewModel
{
    private string _email;
    private string _password;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
    }

    private bool CanLogin()
    {
        return !string.IsNullOrWhiteSpace(Email) && 
               !string.IsNullOrWhiteSpace(Password);
    }

    private async Task LoginAsync()
    {
        IsBusy = true;
        try
        {
            // Login logic
            await AuthService.LoginAsync(Email, Password);
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

### Collection ViewModel
```csharp
public class VehiclesViewModel : BaseViewModel
{
    private ObservableCollection<Vehicle> _vehicles;
    private Vehicle _selectedVehicle;
    private string _searchQuery;

    public ObservableCollection<Vehicle> Vehicles
    {
        get => _vehicles;
        set => SetProperty(ref _vehicles, value);
    }

    public Vehicle SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            SetProperty(ref _selectedVehicle, value);
            if (value != null)
            {
                NavigateToDetailCommand.Execute(value);
            }
        }
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            SetProperty(ref _searchQuery, value);
            SearchCommand.Execute(null);
        }
    }

    public ICommand LoadVehiclesCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand NavigateToDetailCommand { get; }
    public ICommand RefreshCommand { get; }

    public VehiclesViewModel()
    {
        Vehicles = new ObservableCollection<Vehicle>();
        LoadVehiclesCommand = new AsyncRelayCommand(LoadVehiclesAsync);
        SearchCommand = new RelayCommand(Search);
        NavigateToDetailCommand = new AsyncRelayCommand<Vehicle>(NavigateToDetailAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    private async Task LoadVehiclesAsync()
    {
        IsBusy = true;
        try
        {
            var vehicles = await VehicleService.GetAllAsync();
            Vehicles.Clear();
            foreach (var vehicle in vehicles)
            {
                Vehicles.Add(vehicle);
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Search()
    {
        // Implement search logic
    }

    private async Task NavigateToDetailAsync(Vehicle vehicle)
    {
        await Shell.Current.GoToAsync($"VehicleDetail?id={vehicle.Id}");
    }

    private async Task RefreshAsync()
    {
        await LoadVehiclesAsync();
    }
}
```

## 🔗 View-ViewModel Binding

### XAML Binding
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:viewModels="clr-namespace:MyApp.ViewModels"
             x:Class="MyApp.Views.LoginPage"
             x:DataType="viewModels:LoginViewModel">
    
    <ContentPage.BindingContext>
        <viewModels:LoginViewModel />
    </ContentPage.BindingContext>

    <StackLayout Padding="20" Spacing="15">
        <Entry Placeholder="Email"
               Text="{Binding Email, Mode=TwoWay}"
               Keyboard="Email" />
        
        <Entry Placeholder="Password"
               Text="{Binding Password, Mode=TwoWay}"
               IsPassword="True" />
        
        <Button Text="Login"
                Command="{Binding LoginCommand}"
                IsEnabled="{Binding IsBusy, Converter={StaticResource InvertedBoolConverter}}" />
        
        <ActivityIndicator IsRunning="{Binding IsBusy}"
                          IsVisible="{Binding IsBusy}" />
    </StackLayout>
</ContentPage>
```

### Code-Behind Binding
```csharp
public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}
```

## 🔄 Value Converters

### BoolToColorConverter
```csharp
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Colors.Green : Colors.Red;
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

### InvertedBoolConverter
```csharp
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }
}
```

### Register Converters
```xml
<Application.Resources>
    <ResourceDictionary>
        <converters:InvertedBoolConverter x:Key="InvertedBoolConverter" />
        <converters:BoolToColorConverter x:Key="BoolToColorConverter" />
    </ResourceDictionary>
</Application.Resources>
```

## 📱 Navigation Patterns

### Shell Navigation
```csharp
// Navigate to page
await Shell.Current.GoToAsync("PageName");

// Navigate with parameters
await Shell.Current.GoToAsync($"PageName?id={vehicleId}");

// Navigate back
await Shell.Current.GoToAsync("..");

// Navigate to root
await Shell.Current.GoToAsync("//MainPage");
```

### Query Parameters
```csharp
[QueryProperty(nameof(VehicleId), "id")]
public class VehicleDetailViewModel : BaseViewModel
{
    private string _vehicleId;

    public string VehicleId
    {
        get => _vehicleId;
        set
        {
            _vehicleId = value;
            LoadVehicleAsync(value);
        }
    }

    private async Task LoadVehicleAsync(string id)
    {
        // Load vehicle details
    }
}
```

## 🎭 Messaging Pattern

### WeakReferenceMessenger
```csharp
// Send message
WeakReferenceMessenger.Default.Send(new VehicleUpdatedMessage(vehicle));

// Receive message
WeakReferenceMessenger.Default.Register<VehicleUpdatedMessage>(this, (r, m) =>
{
    // Handle message
    RefreshVehicle(m.Vehicle);
});

// Unregister
WeakReferenceMessenger.Default.Unregister<VehicleUpdatedMessage>(this);
```

## 🧪 Testable ViewModels

### Unit Test Example
```csharp
[Fact]
public async Task LoginCommand_WithValidCredentials_NavigatesToMainPage()
{
    // Arrange
    var viewModel = new LoginViewModel();
    viewModel.Email = "test@example.com";
    viewModel.Password = "password123";

    // Act
    await viewModel.LoginCommand.ExecuteAsync();

    // Assert
    Assert.False(viewModel.IsBusy);
    // Verify navigation occurred
}
```

## 📚 Best Practices

### 1. Keep ViewModels Testable
- Don't reference Views directly
- Use interfaces for services
- Inject dependencies

### 2. Use Async Commands
- Always use AsyncRelayCommand for async operations
- Handle exceptions properly
- Show loading states

### 3. Property Change Notifications
- Always use SetProperty for two-way bindings
- Notify dependent properties when needed

### 4. Command CanExecute
- Implement CanExecute for better UX
- Call RaiseCanExecuteChanged when conditions change

### 5. Memory Management
- Unsubscribe from events
- Dispose of resources
- Use weak references for messaging

---

**Remember**: MVVM is about separation of concerns. Keep your Views dumb, ViewModels smart, and Models focused on data.
