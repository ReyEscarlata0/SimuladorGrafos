using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// Dijkstra: caminos mínimos desde nodoInicio hacia todos los demás nodos.
/// Solo válido si todos los pesos son >= 0.
/// </summary>
public class AlgoritmoDijkstra : IAlgoritmoGrafo
{
    public string Nombre => "Dijkstra (Caminos Mínimos)";
    public bool RequiereNodoInicio => true;

    public string[] Codigo => _codigo;

    private static readonly string[] _codigo =
    {
        "void Dijkstra(Grafo grafo, int inicio)",
        "{",
        "    foreach (var n in grafo.Nodos) distancia[n] = ∞;",
        "    distancia[inicio] = 0;",
        "",
        "    while (queden nodos sin fijar)",
        "    {",
        "        u = nodo no fijado con menor distancia[u];",
        "        fijar(u);",
        "",
        "        foreach (var (v, peso) in Vecinos(u))",
        "        {",
        "            nuevaDist = distancia[u] + peso;",
        "            if (nuevaDist < distancia[v])",
        "            {",
        "                distancia[v] = nuevaDist;",
        "                padre[v] = u;",
        "            }",
        "        }",
        "",
        "        // columna del paso para u queda completa",
        "    }",
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
        var visitado = new HashSet<int>();
        foreach (var n in grafo.Nodos) distancia[n.Id] = double.PositiveInfinity;
        distancia[nodoInicio] = 0;

        void AgregarColumna(int uFijado, HashSet<int> fijadosAntes)
        {
            tabla.Columnas.Add($"Paso {tabla.Columnas.Count + 1}");
            foreach (var n in grafo.Nodos)
            {
                CeldaResultado celda;
                if (fijadosAntes.Contains(n.Id))
                    celda = new CeldaResultado { Texto = "*" };
                else if (distancia[n.Id] == double.PositiveInfinity)
                    celda = new CeldaResultado { Texto = "--" };
                else
                    celda = new CeldaResultado
                    {
                        Texto = $"({distancia[n.Id]:0.##}, {(padre.TryGetValue(n.Id, out var p) ? Etiqueta(p) : Etiqueta(nodoInicio))})",
                        Destacada = n.Id == uFijado
                    };
                filaPorNodo[n.Id].Celdas.Add(celda);
            }
        }

        for (int iter = 0; iter < grafo.Nodos.Count; iter++)
        {
            int u = -1;
            double mejor = double.PositiveInfinity;
            foreach (var n in grafo.Nodos)
                if (!visitado.Contains(n.Id) && distancia[n.Id] < mejor) { mejor = distancia[n.Id]; u = n.Id; }
            if (u == -1) break;

            var fijadosAntes = new HashSet<int>(visitado);
            visitado.Add(u);
            pasos.Add(new PasoAnimacion($"Nodo {u} fijado con distancia {distancia[u]}")
                          .ConNodo(u, EstadoNodo.Visitando)
                          .ConLinea(8));

            foreach (var arista in grafo.ObtenerAristasDesde(u))
            {
                int v = grafo.ObtenerVecino(u, arista);
                double nuevaDist = distancia[u] + arista.Peso;
                if (nuevaDist < distancia[v])
                {
                    distancia[v] = nuevaDist;
                    padre[v] = u;
                    pasos.Add(new PasoAnimacion($"Arista {u}-{v} relaja distancia de {v} a {nuevaDist}")
                                  .ConArista(arista.Id, EstadoArista.Explorando)
                                  .ConNodo(v, EstadoNodo.EnCola)
                                  .ConLinea(16));
                }
            }

            AgregarColumna(u, fijadosAntes);
            pasos.Add(new PasoAnimacion($"Nodo {u} terminado")
                          .ConNodo(u, EstadoNodo.Visitado)
                          .ConRevelarIndice(tabla.Columnas.Count - 1)
                          .ConLinea(20));
        }

        tabla.Resumen = $"Distancias mínimas desde {Etiqueta(nodoInicio)}: " + string.Join("  ", grafo.Nodos.Select(n =>
            $"{n.Etiqueta}={(distancia[n.Id] == double.PositiveInfinity ? "∞" : distancia[n.Id].ToString("0.##"))}"));
        return new ResultadoAlgoritmo(pasos, tabla);
    }
}
