using System.Globalization;

namespace AUTORENT.Converters
{
    public class BusyTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isBusy)
            {
                return isBusy ? "Iniciando sesión..." : "Iniciar Sesión";
            }
            return "Iniciar Sesión";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
