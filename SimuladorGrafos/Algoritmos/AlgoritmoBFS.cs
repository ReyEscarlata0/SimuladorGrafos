using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// BFS (Recorrido en Anchura / Breadth-First Search).
/// Explora el grafo "por capas": primero todos los vecinos directos del nodo
/// de inicio, después los vecinos de esos vecinos, etc. Se implementa con una
/// cola (Queue&lt;int&gt;).
/// </summary>
public class AlgoritmoBFS : IAlgoritmoGrafo
{
    public string Nombre => "BFS (Recorrido en Anchura)";
    public bool RequiereNodoInicio => true;

    public string[] Codigo => _codigo;

    private static readonly string[] _codigo =
    {
        "void BFS(Grafo grafo, int inicio)",
        "{",
        "    var visitados = new HashSet<int>();",
        "    var cola = new Queue<int>();",
        "",
        "    cola.Enqueue(inicio);",
        "    visitados.Add(inicio);",
        "",
        "    while (cola.Count > 0)",
        "    {",
        "        int actual = cola.Dequeue();",
        "        Visitar(actual);",
        "",
        "        foreach (var arista in AristasDesde(actual))",
        "        {",
        "            int vecino = Vecino(arista, actual);",
        "            if (!visitados.Contains(vecino))",
        "            {",
        "                visitados.Add(vecino);",
        "                cola.Enqueue(vecino);",
        "            }",
        "        }",
        "",
        "        // actual queda completamente procesado",
        "    }",
        "}",
    };

    public ResultadoAlgoritmo Ejecutar(Grafo grafo, int nodoInicio)
    {
        var pasos = new List<PasoAnimacion>();
        var tabla = new TablaResultado
        {
            TituloPrimeraColumna = "Nodo",
            Orientacion = OrientacionTabla.PorFilas
        };
        tabla.Columnas.Add("Orden");
        tabla.Columnas.Add("Descubierto desde");

        string Etiqueta(int id) => grafo.Nodos.First(n => n.Id == id).Etiqueta;

        var padre = new Dictionary<int, int?> { [nodoInicio] = null };
        var cola = new Queue<int>();
        var visitados = new HashSet<int>();
        cola.Enqueue(nodoInicio);
        visitados.Add(nodoInicio);
        pasos.Add(new PasoAnimacion($"Nodo {nodoInicio} agregado a la cola")
                      .ConNodo(nodoInicio, EstadoNodo.EnCola)
                      .ConLinea(5));

        while (cola.Count > 0)
        {
            int actual = cola.Dequeue();
            pasos.Add(new PasoAnimacion($"Visitando nodo {actual}")
                          .ConNodo(actual, EstadoNodo.Visitando)
                          .ConLinea(11));

            foreach (var arista in grafo.ObtenerAristasDesde(actual))
            {
                int vecino = grafo.ObtenerVecino(actual, arista);
                if (!visitados.Contains(vecino))
                {
                    visitados.Add(vecino);
                    padre[vecino] = actual;
                    cola.Enqueue(vecino);
                    pasos.Add(new PasoAnimacion($"Arista {actual}-{vecino} explorada, nodo {vecino} agregado a la cola")
                                  .ConArista(arista.Id, EstadoArista.Explorando)
                                  .ConNodo(vecino, EstadoNodo.EnCola)
                                  .ConLinea(19));
                }
            }

            var fila = new FilaResultado(Etiqueta(actual));
            fila.Celdas.Add(new CeldaResultado { Texto = (tabla.Filas.Count + 1).ToString(), Destacada = true });
            fila.Celdas.Add(new CeldaResultado { Texto = padre[actual].HasValue ? Etiqueta(padre[actual]!.Value) : "Inicio" });
            tabla.Filas.Add(fila);

            pasos.Add(new PasoAnimacion($"Nodo {actual} terminado")
                          .ConNodo(actual, EstadoNodo.Visitado)
                          .ConRevelarIndice(tabla.Filas.Count - 1)
                          .ConLinea(23));
        }

        tabla.Resumen = $"Recorrido completo ({tabla.Filas.Count} nodos): {string.Join(" → ", tabla.Filas.Select(f => f.Etiqueta))}";
        return new ResultadoAlgoritmo(pasos, tabla);
    }
}
