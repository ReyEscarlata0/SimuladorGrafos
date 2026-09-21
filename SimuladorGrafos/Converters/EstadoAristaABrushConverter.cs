using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SimuladorGrafos.Models;

namespace SimuladorGrafos.Converters;

// Lo mismo que el converter de nodos, pero para las líneas (aristas) que conectan los nodos.
// Colorea cada línea según su papel en el algoritmo: normal, en revisión, o ya elegida como parte del resultado.
public class EstadoAristaABrushConverter : IValueConverter
{
    // Los colores deben coincidir con los brushes AristaXxxBrush definidos en App.xaml.
    private static readonly Brush Normal = new SolidColorBrush(Color.FromRgb(0x54, 0x5A, 0x72));      // gris: arista normal, sin usar todavía
    private static readonly Brush Explorando = new SolidColorBrush(Color.FromRgb(0xFF, 0x9F, 0x43));  // naranja: arista que se está evaluando ahora
    private static readonly Brush Incluida = new SolidColorBrush(Color.FromRgb(0x22, 0xD3, 0xAC));    // verde: arista que forma parte del resultado final (camino/árbol)

    static EstadoAristaABrushConverter()
    {
        Normal.Freeze();
        Explorando.Freeze();
        Incluida.Freeze();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var estado = value is EstadoArista e ? e : EstadoArista.Normal;
        return estado switch
        {
            EstadoArista.Normal => Normal,
            EstadoArista.Explorando => Explorando,
            EstadoArista.Incluida => Incluida,
            _ => Normal
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
