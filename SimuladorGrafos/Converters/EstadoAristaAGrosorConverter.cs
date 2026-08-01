using System.Globalization;
using System.Windows.Data;
using SimuladorGrafos.Models;

namespace SimuladorGrafos.Converters;

public class EstadoAristaAGrosorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var estado = value is EstadoArista e ? e : EstadoArista.Normal;
        return estado == EstadoArista.Incluida ? 3.5 : 1.5;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
