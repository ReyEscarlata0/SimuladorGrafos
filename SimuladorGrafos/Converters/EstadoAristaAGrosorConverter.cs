using System.Globalization;
using System.Windows.Data;
using SimuladorGrafos.Models;

namespace SimuladorGrafos.Converters;

// Hace que las líneas (aristas) que forman parte del resultado final se vean más gruesas que las demás,
// para que resalten visualmente sobre el resto del grafo.
public class EstadoAristaAGrosorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var estado = value is EstadoArista e ? e : EstadoArista.Normal;
        // Si la arista es parte del resultado (Incluida) -> línea gruesa (3.5). Si no -> línea fina (1.5).
        return estado == EstadoArista.Incluida ? 3.5 : 1.5;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
