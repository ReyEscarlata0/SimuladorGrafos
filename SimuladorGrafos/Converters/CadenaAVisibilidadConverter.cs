using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimuladorGrafos.Converters;

// Muestra un elemento SOLO si hay texto real escrito (no vacío ni solo espacios).
// Ejemplo: mostrar un mensaje de error solo cuando ese mensaje tiene contenido.
public class CadenaAVisibilidadConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        // Si el texto tiene contenido -> Visible. Si está vacío o son solo espacios -> Collapsed.
        !string.IsNullOrWhiteSpace(value as string) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
