using SimuladorGrafos.Models;

namespace SimuladorGrafos.ViewModels;

/// Wrapper bindable de Nodo: expone posición ya ajustada para Canvas.Left/Top
/// y el Estado (que la UI convierte a color mediante EstadoNodoABrushConverter).
public class NodoVista : ObservableObject
{
    public const double Radio = 20;

    public Nodo Modelo { get; }

    public int Id => Modelo.Id;
    public string Etiqueta => Modelo.Etiqueta;

    public double Left => Modelo.X - Radio;
    public double Top => Modelo.Y - Radio;

    private EstadoNodo _estado;

    public EstadoNodo Estado
    {
        get => _estado;
        set
        {
            if (EstablecerPropiedad(ref _estado, value))
                Modelo.Estado = value;
        }
    }

    public NodoVista(Nodo modelo)
    {
        Modelo = modelo;
        _estado = modelo.Estado;
    }

    /// Mueve el nodo a una nueva posición (centro) y notifica a la UI y a
    /// las aristas conectadas para que se redibujen.
    public void MoverA(double x, double y)
    {
        Modelo.X = x;
        Modelo.Y = y;
        Notificar(nameof(Left));
        Notificar(nameof(Top));
    }
}
