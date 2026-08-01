namespace SimuladorGrafos.ViewModels;

/// <summary>
/// Una línea del panel "Código en vivo". Numero es 1-based (para mostrar),
/// Indice es 0-based (coincide con PasoAnimacion.LineaResaltada).
/// </summary>
public class LineaCodigoVista : ObservableObject
{
    public int Indice { get; }
    public int Numero => Indice + 1;
    public string Texto { get; }
    public bool EsComentario => Texto.TrimStart().StartsWith("//");

    private bool _esActual;
    public bool EsActual
    {
        get => _esActual;
        set => EstablecerPropiedad(ref _esActual, value);
    }

    public LineaCodigoVista(int indice, string texto)
    {
        Indice = indice;
        Texto = texto;
    }
}
