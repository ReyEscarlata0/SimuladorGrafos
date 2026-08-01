using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// Bellman-Ford: caminos mínimos desde nodoInicio, igual que Dijkstra pero
/// SÍ soporta pesos negativos. Relaja TODAS las aristas del grafo, repitiendo
/// (NroNodos - 1) veces.
/// </summary>
public class AlgoritmoBellmanFord : IAlgoritmoGrafo
{
    public string Nombre => "Bellman-Ford (Caminos Mínimos)";
    public bool RequiereNodoInicio => true;

    public string[] Codigo => _codigo;

    private static readonly string[] _codigo =
    {
        "void BellmanFord(Grafo grafo, int inicio)",
        "{",
        "    foreach (var n in grafo.Nodos) distancia[n] = ∞;",
        "    distancia[inicio] = 0;",
        "",
        "    for (ronda = 1; ronda < NroNodos; ronda++)",
        "    {",
        "        huboCambio = false;",
        "",
        "        foreach (var arista in grafo.Aristas)",
        "        {",
        "            if (Relajar(arista.Origen, arista.Destino, arista.Peso))",
        "                huboCambio = true;",
        "            if (!arista.EsDirigida)",
        "                if (Relajar(arista.Destino, arista.Origen, arista.Peso))",
        "                    huboCambio = true;",
        "        }",
        "",
        "        // columna de la ronda queda completa",
        "",
        "        if (!huboCambio) break;",
        "    }",
        "",
        "    // distancias finales calculadas",
        "}",
        "",
        "bool Relajar(int u, int v, double peso)",
        "{",
        "    nuevaDist = distancia[u] + peso;",
        "    if (nuevaDist < distancia[v])",
        "    {",
        "        distancia[v] = nuevaDist;",
        "        padre[v] = u;",
        "        return true;",
        "    }",
        "    return false;",
        "}",
    };

    public ResultadoAlgoritmo Ejecutar(Grafo grafo, int nodoInicio)
    {
        var pasos = new List<PasoAnimacion>();
        var tabla = new TablaResultado
        {
            TituloPrimeraColumna = "Vértice",
            Orientacion = OrientacionTabla.PorColumnas
        };

        string Etiqueta(int id) => grafo.Nodos.First(n => n.Id == id).Etiqueta;
        var filaPorNodo = new Dictionary<int, FilaResultado>();
        foreach (var n in grafo.Nodos)
        {
            var fila = new FilaResultado(n.Etiqueta);
            filaPorNodo[n.Id] = fila;
            tabla.Filas.Add(fila);
        }

        var distancia = new Dictionary<int, double>();
        var padre = new Dictionary<int, int>();
        foreach (var n in grafo.Nodos) distancia[n.Id] = double.PositiveInfinity;
        distancia[nodoInicio] = 0;
        pasos.Add(new PasoAnimacion($"Nodo {nodoInicio} distancia inicial 0")
                      .ConNodo(nodoInicio, EstadoNodo.Visitando)
                      .ConLinea(3));

        var cambiadosEsteRonda = new HashSet<int>();

        bool Relajar(int u, int v, Arista arista)
        {
            if (distancia[u] == double.PositiveInfinity) return false;
            double nuevaDist = distancia[u] + arista.Peso;
            if (nuevaDist < distancia[v])
            {
                distancia[v] = nuevaDist;
                padre[v] = u;
                cambiadosEsteRonda.Add(v);
                pasos.Add(new PasoAnimacion($"Arista {u}-{v} relaja distancia de {v} a {nuevaDist}")
                              .ConArista(arista.Id, EstadoArista.Explorando)
                              .ConNodo(v, EstadoNodo.EnCola)
                              .ConLinea(31));
                return true;
            }
            return false;
        }

        void AgregarColumna(int numeroRonda)
        {
            tabla.Columnas.Add($"Ronda {numeroRonda}");
            foreach (var n in grafo.Nodos)
            {
                CeldaResultado celda = distancia[n.Id] == double.PositiveInfinity
                    ? new CeldaResultado { Texto = "--" }
                    : new CeldaResultado
                    {
                        Texto = $"({distancia[n.Id]:0.##}, {(padre.TryGetValue(n.Id, out var p) ? Etiqueta(p) : Etiqueta(nodoInicio))})",
                        Destacada = cambiadosEsteRonda.Contains(n.Id)
                    };
                filaPorNodo[n.Id].Celdas.Add(celda);
            }
        }

        int totalNodos = grafo.Nodos.Count;
        for (int ronda = 0; ronda < totalNodos - 1; ronda++)
        {
            cambiadosEsteRonda.Clear();
            bool huboCambio = false;
            foreach (var arista in grafo.Aristas)
            {
                huboCambio |= Relajar(arista.OrigenId, arista.DestinoId, arista);
                if (!arista.EsDirigida)
                    huboCambio |= Relajar(arista.DestinoId, arista.OrigenId, arista);
            }

            AgregarColumna(ronda + 1);
            pasos.Add(new PasoAnimacion($"Ronda {ronda + 1} completa")
                          .ConRevelarIndice(tabla.Columnas.Count - 1)
                          .ConLinea(18));

            if (!huboCambio) break;
        }

        foreach (var n2 in grafo.Nodos)
            if (distancia[n2.Id] != double.PositiveInfinity)
                pasos.Add(new PasoAnimacion($"Nodo {n2.Id} distancia final {distancia[n2.Id]}")
                              .ConNodo(n2.Id, EstadoNodo.Visitado)
                              .ConLinea(23));

        tabla.Resumen = $"Distancias mínimas desde {Etiqueta(nodoInicio)}: " + string.Join("  ", grafo.Nodos.Select(n =>
            $"{n.Etiqueta}={(distancia[n.Id] == double.PositiveInfinity ? "∞" : distancia[n.Id].ToString("0.##"))}"));
        return new ResultadoAlgoritmo(pasos, tabla);
    }
}
