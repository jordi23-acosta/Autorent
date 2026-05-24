using AUTORENT.ViewModels;

namespace AUTORENT
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel();
        }

        private void OnConductorTapped(object sender, TappedEventArgs e)
        {
            if (BindingContext is RegisterViewModel vm)
            {
                vm.SelectedUserTypeIndex = 0;
            }
        }

        private void OnPropietarioTapped(object sender, TappedEventArgs e)
        {
            if (BindingContext is RegisterViewModel vm)
            {
                vm.SelectedUserTypeIndex = 1;
            }
        }
    }
}
