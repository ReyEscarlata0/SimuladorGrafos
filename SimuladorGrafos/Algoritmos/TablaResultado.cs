namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// Cómo se "revela" la tabla a medida que avanza la animación:
/// - PorColumnas: la tabla ya tiene todas las filas (ej. todos los vértices) y
///   se van agregando columnas de a una (ej. "Paso 1", "Paso 2"...). Es el
///   formato clásico de Dijkstra/Prim/Bellman-Ford (vértice x iteración).
/// - PorFilas: las columnas son fijas y se van agregando filas de a una (ej.
///   BFS/DFS agregan una fila por nodo visitado; Kruskal, una por arista evaluada).
/// </summary>
public enum OrientacionTabla
{
    PorColumnas,
    PorFilas
}

/// <summary>
/// Una celda de la tabla de resultados: el texto a mostrar y si debe
/// resaltarse (ej. el vértice recién fijado en ese paso).
/// </summary>
public class CeldaResultado
{
    public string Texto { get; set; } = "—";
    public bool Destacada { get; set; }
}

/// <summary>
/// Una fila de la tabla (ej. un vértice o una arista), con una celda por
/// cada columna de datos de TablaResultado.Columnas.
/// </summary>
public class FilaResultado
{
    public string Etiqueta { get; }
    public List<CeldaResultado> Celdas { get; } = new();

    public FilaResultado(string etiqueta)
    {
        Etiqueta = etiqueta;
    }
}

/// <summary>
/// Tabla de resultados propia de cada algoritmo (además de la animación
/// visual sobre el grafo). Se muestra tal cual, sin lógica de UI.
/// </summary>
public class TablaResultado
{
    public string TituloPrimeraColumna { get; init; } = "Vértice";
    public OrientacionTabla Orientacion { get; init; } = OrientacionTabla.PorColumnas;
    public List<string> Columnas { get; } = new();
    public List<FilaResultado> Filas { get; } = new();

    /// Resultado final en una línea (ej. distancias finales, peso del MST,
    /// orden de recorrido completo), para mostrar debajo de la tabla paso a paso.
    public string? Resumen { get; set; }
}

/// <summary>
/// Lo que devuelve cada algoritmo: los pasos para animar el grafo (como antes)
/// más, opcionalmente, su propia tabla de resultados.
/// </summary>
public class ResultadoAlgoritmo
{
    public List<PasoAnimacion> Pasos { get; }
    public TablaResultado? Tabla { get; }

    public ResultadoAlgoritmo(List<PasoAnimacion> pasos, TablaResultado? tabla = null)
    {
        Pasos = pasos;
        Tabla = tabla;
    }
}
