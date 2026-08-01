using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using SimuladorGrafos.Models;

namespace SimuladorGrafos.ViewModels;

/// Wrapper bindable de Arista: calcula las coordenadas de la línea y del
/// triángulo de la flecha a partir de la posición de sus dos NodoVista.
public class AristaVista : ObservableObject
{
    private const double LargoFlecha = 12;
    private const double AnguloFlecha = Math.PI / 8;

    public Arista Modelo { get; }
    public NodoVista Origen { get; }
    public NodoVista Destino { get; }

    public int Id => Modelo.Id;
    public double Peso => Modelo.Peso;
    public bool EsDirigida => Modelo.EsDirigida;

    public double X1 => Origen.Left + NodoVista.Radio;
    public double Y1 => Origen.Top + NodoVista.Radio;
    public double X2 => Destino.Left + NodoVista.Radio;
    public double Y2 => Destino.Top + NodoVista.Radio;

    public double XEtiqueta => (X1 + X2) / 2;
    public double YEtiqueta => (Y1 + Y2) / 2;

    /// Punto sobre el borde del círculo destino (no en su centro), para que
    /// la punta de la flecha quede visible en vez de escondida bajo el nodo.
    public Point PuntoFinal
    {
        get
        {
            double dx = X2 - X1, dy = Y2 - Y1;
            double distancia = Math.Sqrt(dx * dx + dy * dy);
            if (distancia < 0.001) return new Point(X2, Y2);
            double factor = (distancia - NodoVista.Radio) / distancia;
            return new Point(X1 + dx * factor, Y1 + dy * factor);
        }
    }

    public PointCollection PuntosFlecha
    {
        get
        {
            var punta = PuntoFinal;
            double angulo = Math.Atan2(punta.Y - Y1, punta.X - X1);
            var p1 = new Point(
                punta.X - LargoFlecha * Math.Cos(angulo - AnguloFlecha),
                punta.Y - LargoFlecha * Math.Sin(angulo - AnguloFlecha));
            var p2 = new Point(
                punta.X - LargoFlecha * Math.Cos(angulo + AnguloFlecha),
                punta.Y - LargoFlecha * Math.Sin(angulo + AnguloFlecha));
            return new PointCollection { punta, p1, p2 };
        }
    }

    private EstadoArista _estado;

    public EstadoArista Estado
    {
        get => _estado;
        set
        {
            if (EstablecerPropiedad(ref _estado, value))
                Modelo.Estado = value;
        }
    }

    public AristaVista(Arista modelo, NodoVista origen, NodoVista destino)
    {
        Modelo = modelo;
        Origen = origen;
        Destino = destino;
        _estado = modelo.Estado;

        Origen.PropertyChanged += NodoConectado_PropertyChanged;
        Destino.PropertyChanged += NodoConectado_PropertyChanged;
    }

    /// Cuando cualquiera de los dos nodos conectados se mueve (arrastre),
    /// recalcula la línea, la flecha y la etiqueta de esta arista.
    private void NodoConectado_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(NodoVista.Left) or nameof(NodoVista.Top))) return;

        Notificar(nameof(X1));
        Notificar(nameof(Y1));
        Notificar(nameof(X2));
        Notificar(nameof(Y2));
        Notificar(nameof(XEtiqueta));
        Notificar(nameof(YEtiqueta));
        Notificar(nameof(PuntosFlecha));
    }
}
