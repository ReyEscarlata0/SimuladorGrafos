using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimuladorGrafos.Converters;

// Es lo contrario del anterior: muestra el elemento SOLO cuando NO hay nada seleccionado.
// Ejemplo: mostrar el mensaje "Selecciona un nodo para ver detalles" cuando todavía no se ha elegido ninguno.
public class EsNuloAVisibilidadConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        // Si NO hay valor (es null) -> Visible. Si ya hay algo elegido -> Collapsed.
        value == null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
