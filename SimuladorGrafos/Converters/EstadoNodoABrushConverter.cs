using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SimuladorGrafos.Models;

namespace SimuladorGrafos.Converters;

public class EstadoNodoABrushConverter : IValueConverter
{
    // Los colores deben coincidir con los brushes NodoXxxBrush definidos en App.xaml.
    private static readonly Brush SinVisitar = new SolidColorBrush(Color.FromRgb(0xE4, 0xE7, 0xF0));
    private static readonly Brush EnCola = new SolidColorBrush(Color.FromRgb(0x4C, 0x8D, 0xFF));
    private static readonly Brush Visitando = new SolidColorBrush(Color.FromRgb(0xFF, 0xC2, 0x4B));
    private static readonly Brush Visitado = new SolidColorBrush(Color.FromRgb(0x22, 0xD3, 0xAC));

    static EstadoNodoABrushConverter()
    {
        SinVisitar.Freeze();
        EnCola.Freeze();
        Visitando.Freeze();
        Visitado.Freeze();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var estado = value is EstadoNodo e ? e : EstadoNodo.SinVisitar;
        return estado switch
        {
            EstadoNodo.SinVisitar => SinVisitar,
            EstadoNodo.EnCola => EnCola,
            EstadoNodo.Visitando => Visitando,
            EstadoNodo.Visitado => Visitado,
            _ => SinVisitar
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
