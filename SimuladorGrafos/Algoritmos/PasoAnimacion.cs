using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// Un "fotograma" de la animación: qué texto mostrar en el log y qué nodos y
/// aristas cambian de color en este paso. Los algoritmos SOLO producen datos
/// (List&lt;PasoAnimacion&gt;) y no tocan la interfaz directamente — el
/// ReproductorAnimacion (Services/) es quien después aplica estos cambios a
/// la UI, uno por uno, con un temporizador.
/// </summary>
public class PasoAnimacion
{
    public string Descripcion { get; }
    public List<(int NodoId, EstadoNodo NuevoEstado)> CambiosNodo { get; } = new();
    public List<(int AristaId, EstadoArista NuevoEstado)> CambiosArista { get; } = new();

    /// Si tiene valor, indica que al aplicar este paso también debe "revelarse"
    /// la columna (o fila, según TablaResultado.Orientacion) con ese índice
    /// en la tabla de resultados del algoritmo.
    public int? RevelarIndice { get; private set; }

    /// Índice (0-based) de la línea del pseudocódigo (IAlgoritmoGrafo.Codigo)
    /// que se está "ejecutando" en este paso, para resaltarla en el panel de código.
    public int? LineaResaltada { get; private set; }

    public PasoAnimacion(string descripcion)
    {
        Descripcion = descripcion;
    }

    public PasoAnimacion ConNodo(int nodoId, EstadoNodo estado)
    {
        CambiosNodo.Add((nodoId, estado));
        return this;
    }

    public PasoAnimacion ConArista(int aristaId, EstadoArista estado)
    {
        CambiosArista.Add((aristaId, estado));
        return this;
    }

    public PasoAnimacion ConRevelarIndice(int indice)
    {
        RevelarIndice = indice;
        return this;
    }

    public PasoAnimacion ConLinea(int lineaIndice)
    {
        LineaResaltada = lineaIndice;
        return this;
    }
}
