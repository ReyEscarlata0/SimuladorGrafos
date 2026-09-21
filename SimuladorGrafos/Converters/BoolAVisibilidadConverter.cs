using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimuladorGrafos.Converters;

// Convierte un true/false en "mostrar" o "esconder" un elemento visual.
// Ejemplo de uso: si una casilla de "Mostrar ayuda" está marcada (true), el panel de ayuda se hace visible.
public class BoolAVisibilidadConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        // Si el valor es true -> Visible (se ve en pantalla). Si es false -> Collapsed (desaparece y no ocupa espacio).
        value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
