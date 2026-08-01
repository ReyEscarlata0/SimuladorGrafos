using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// Kruskal: Árbol de Expansión Mínima (MST), pero en vez de crecer desde un
/// nodo, se ordenan TODAS las aristas de menor a mayor peso y se van
/// agregando de una en una, siempre que no formen un ciclo. Por eso no
/// necesita nodo de inicio (RequiereNodoInicio = false).
/// Usa Union-Find (Conjuntos Disjuntos / DSU) para detectar ciclos.
/// </summary>
public class AlgoritmoKruskal : IAlgoritmoGrafo
{
    public string Nombre => "Kruskal (Árbol de Expansión Mínima)";
    public bool RequiereNodoInicio => false;

    public string[] Codigo => _codigo;

    private static readonly string[] _codigo =
    {
        "void Kruskal(Grafo grafo)",
        "{",
        "    foreach (var n in grafo.Nodos) padre[n] = n;   // Union-Find",
        "    ordenadas = grafo.Aristas.OrderBy(a => a.Peso);",
        "",
        "    foreach (var arista in ordenadas)",
        "    {",
        "        (origen, destino) = (arista.Origen, arista.Destino);",
        "",
        "        if (Encontrar(origen) != Encontrar(destino))",
        "        {",
        "            Unir(origen, destino);",
        "            incluidas.Add(arista);",
        "            // arista incluida: no forma ciclo",
        "        }",
        "        else",
        "        {",
        "            // arista descartada: formaría un ciclo",
        "        }",
        "    }",
        "}",
    };

    public ResultadoAlgoritmo Ejecutar(Grafo grafo, int nodoInicio)
    {
        var pasos = new List<PasoAnimacion>();
        var tabla = new TablaResultado
        {
            TituloPrimeraColumna = "Arista",
            Orientacion = OrientacionTabla.PorFilas
        };
        tabla.Columnas.Add("Peso");
        tabla.Columnas.Add("Resultado");

        string Etiqueta(int id) => grafo.Nodos.First(n => n.Id == id).Etiqueta;

        var padre = new Dictionary<int, int>();
        foreach (var n in grafo.Nodos) padre[n.Id] = n.Id;

        int Encontrar(int x)
        {
            while (padre[x] != x) x = padre[x];
            return x;
        }

        bool Unir(int a, int b)
        {
            int raizA = Encontrar(a), raizB = Encontrar(b);
            if (raizA == raizB) return false;
            padre[raizA] = raizB;
            return true;
        }

        var ordenadas = grafo.Aristas.OrderBy(a => a.Peso).ToList();
        var incluidas = new List<Arista>();

        foreach (var arista in ordenadas)
        {
            pasos.Add(new PasoAnimacion($"Evaluando arista {arista.OrigenId}-{arista.DestinoId} (peso {arista.Peso})")
                          .ConArista(arista.Id, EstadoArista.Explorando)
                          .ConLinea(7));

            var fila = new FilaResultado($"{Etiqueta(arista.OrigenId)}-{Etiqueta(arista.DestinoId)}");
            fila.Celdas.Add(new CeldaResultado { Texto = arista.Peso.ToString("0.##") });

            if (Unir(arista.OrigenId, arista.DestinoId))
            {
                incluidas.Add(arista);
                fila.Celdas.Add(new CeldaResultado { Texto = "Incluida", Destacada = true });
                tabla.Filas.Add(fila);
                pasos.Add(new PasoAnimacion($"Arista {arista.OrigenId}-{arista.DestinoId} incluida en el MST (no forma ciclo)")
                              .ConArista(arista.Id, EstadoArista.Incluida)
                              .ConNodo(arista.OrigenId, EstadoNodo.Visitado)
                              .ConNodo(arista.DestinoId, EstadoNodo.Visitado)
                              .ConRevelarIndice(tabla.Filas.Count - 1)
                              .ConLinea(12));
            }
            else
            {
                fila.Celdas.Add(new CeldaResultado { Texto = "Descartada" });
                tabla.Filas.Add(fila);
                pasos.Add(new PasoAnimacion($"Arista {arista.OrigenId}-{arista.DestinoId} descartada (formaría un ciclo)")
                              .ConArista(arista.Id, EstadoArista.Normal)
                              .ConRevelarIndice(tabla.Filas.Count - 1)
                              .ConLinea(17));
            }
        }

        double pesoTotal = incluidas.Sum(a => a.Peso);
        string listado = string.Join(", ", incluidas.Select(a => $"{Etiqueta(a.OrigenId)}-{Etiqueta(a.DestinoId)}"));
        tabla.Resumen = $"Árbol de expansión mínima — peso total: {pesoTotal:0.##} · aristas: {listado}";
        return new ResultadoAlgoritmo(pasos, tabla);
    }
}
