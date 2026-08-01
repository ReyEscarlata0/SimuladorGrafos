namespace SimuladorGrafos.Models;

/// <summary>
/// Estructura de datos del grafo (nodos + aristas) y helpers de navegación.
/// No contiene lógica de ningún algoritmo: solo permite consultar vecinos,
/// que es lo que van a usar BFS/DFS/Dijkstra/Bellman-Ford/Prim/Kruskal.
/// </summary>
public class Grafo
{
    public List<Nodo> Nodos { get; } = new();
    public List<Arista> Aristas { get; } = new();

    private int _siguienteNodoId;
    private int _siguienteAristaId;

    public Nodo AgregarNodo(double x, double y, string? etiqueta = null)
    {
        int id = _siguienteNodoId++;
        var nodo = new Nodo(id, etiqueta ?? id.ToString(), x, y);
        Nodos.Add(nodo);
        return nodo;
    }

    public Arista AgregarArista(int origenId, int destinoId, double peso, bool esDirigida)
    {
        int id = _siguienteAristaId++;
        var arista = new Arista(id, origenId, destinoId, peso, esDirigida);
        Aristas.Add(arista);
        return arista;
    }

    public void Limpiar()
    {
        Nodos.Clear();
        Aristas.Clear();
        _siguienteNodoId = 0;
        _siguienteAristaId = 0;
    }

    public void ReiniciarEstados()
    {
        foreach (var nodo in Nodos) nodo.Estado = EstadoNodo.SinVisitar;
        foreach (var arista in Aristas) arista.Estado = EstadoArista.Normal;
    }

    /// Aristas que "salen" de un nodo. Si la arista no es dirigida, también
    /// cuenta como saliente en ambos sentidos (para poder recorrerla en cualquier dirección).
    public IEnumerable<Arista> ObtenerAristasDesde(int nodoId) =>
        Aristas.Where(a => a.OrigenId == nodoId || (!a.EsDirigida && a.DestinoId == nodoId));

    /// Dado un nodo y una arista que sale de él (ver ObtenerAristasDesde), devuelve el otro extremo.
    public int ObtenerVecino(int nodoId, Arista arista) =>
        arista.OrigenId == nodoId ? arista.DestinoId : arista.OrigenId;

    public IEnumerable<int> ObtenerVecinos(int nodoId) =>
        ObtenerAristasDesde(nodoId).Select(a => ObtenerVecino(nodoId, a));
}
