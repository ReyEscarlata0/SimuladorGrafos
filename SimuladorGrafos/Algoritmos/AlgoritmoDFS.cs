using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// DFS (Recorrido en Profundidad / Depth-First Search).
/// A diferencia de BFS, se mete lo más profundo posible por un camino antes
/// de retroceder ("backtrack"). Implementado en forma recursiva.
/// </summary>
public class AlgoritmoDFS : IAlgoritmoGrafo
{
    public string Nombre => "DFS (Recorrido en Profundidad)";
    public bool RequiereNodoInicio => true;

    public string[] Codigo => _codigo;

    private static readonly string[] _codigo =
    {
        "void DFS(Grafo grafo, int inicio) => Visitar(inicio, null);",
        "",
        "void Visitar(int actual, int? padre)",
        "{",
        "    visitados.Add(actual);",
        "    RegistrarEnRecorrido(actual, padre);",
        "",
        "    foreach (var arista in AristasDesde(actual))",
        "    {",
        "        int vecino = Vecino(arista, actual);",
        "        if (!visitados.Contains(vecino))",
        "        {",
        "            ExplorarArista(actual, vecino);",
        "            Visitar(vecino, actual);",
        "        }",
        "    }",
        "",
        "    // actual queda completamente procesado",
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

        var visitados = new HashSet<int>();

        void Visitar(int actual, int? padre)
        {
            visitados.Add(actual);
            pasos.Add(new PasoAnimacion($"Visitando nodo {actual}")
                          .ConNodo(actual, EstadoNodo.Visitando)
                          .ConLinea(4));

            var fila = new FilaResultado(Etiqueta(actual));
            fila.Celdas.Add(new CeldaResultado { Texto = (tabla.Filas.Count + 1).ToString(), Destacada = true });
            fila.Celdas.Add(new CeldaResultado { Texto = padre.HasValue ? Etiqueta(padre.Value) : "Inicio" });
            tabla.Filas.Add(fila);
            pasos.Add(new PasoAnimacion($"Nodo {actual} agregado al recorrido")
                          .ConRevelarIndice(tabla.Filas.Count - 1)
                          .ConLinea(5));

            foreach (var arista in grafo.ObtenerAristasDesde(actual))
            {
                int vecino = grafo.ObtenerVecino(actual, arista);
                if (!visitados.Contains(vecino))
                {
                    pasos.Add(new PasoAnimacion($"Arista {actual}-{vecino} explorada")
                                  .ConArista(arista.Id, EstadoArista.Explorando)
                                  .ConNodo(vecino, EstadoNodo.EnCola)
                                  .ConLinea(12));
                    Visitar(vecino, actual);
                }
            }

            pasos.Add(new PasoAnimacion($"Nodo {actual} terminado")
                          .ConNodo(actual, EstadoNodo.Visitado)
                          .ConLinea(17));
        }

        Visitar(nodoInicio, null);
        tabla.Resumen = $"Recorrido completo ({tabla.Filas.Count} nodos): {string.Join(" → ", tabla.Filas.Select(f => f.Etiqueta))}";
        return new ResultadoAlgoritmo(pasos, tabla);
    }
}
