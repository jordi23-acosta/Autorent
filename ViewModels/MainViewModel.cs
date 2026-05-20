using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace AutoRent.ViewModels;

public class MainViewModel : BaseViewModel
{
    public ICommand GoToCarsCommand { get; }

    public MainViewModel()
    {
        GoToCarsCommand = new Command(async () => await Shell.Current.GoToAsync("/cars"));
    }
}
