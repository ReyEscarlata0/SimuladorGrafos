using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimuladorGrafos.Converters;

// Muestra un elemento SOLO si hay algo seleccionado (el dato no está vacío).
// Ejemplo: mostrar el panel de "detalles del nodo" solo cuando el usuario ha elegido un nodo.
public class NuloAVisibilidadConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        // Si hay un valor (no es null) -> Visible. Si no hay nada seleccionado -> Collapsed.
        value != null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
