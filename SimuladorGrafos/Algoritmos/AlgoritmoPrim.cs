using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

/// <summary>
/// Prim: Árbol de Expansión Mínima (MST). Arranca desde nodoInicio y va
/// "creciendo" el árbol agregando siempre la arista más barata que conecta
/// un nodo YA incluido con uno que todavía NO está incluido.
/// Nota: Prim/Kruskal ignoran la dirección de las aristas (EsDirigida) —
/// se tratan como un grafo no dirigido.
/// </summary>
public class AlgoritmoPrim : IAlgoritmoGrafo
{
    public string Nombre => "Prim (Árbol de Expansión Mínima)";
    public bool RequiereNodoInicio => true;

    public string[] Codigo => _codigo;

    private static readonly string[] _codigo =
    {
        "void Prim(Grafo grafo, int inicio)",
        "{",
        "    foreach (var n in grafo.Nodos) clave[n] = ∞;",
        "    clave[inicio] = 0;",
        "",
        "    while (queden nodos sin incluir)",
        "    {",
        "        u = nodo no incluido con menor clave[u];",
        "        incluido.Add(u);",
        "",
        "        if (aristaElegida[u] existe)",
        "            marcarComoIncluidaEnMST(aristaElegida[u]);",
        "",
        "        foreach (var (v, peso, arista) in Vecinos(u))",
        "        {",
        "            if (!incluido.Contains(v) && peso < clave[v])",
        "            {",
        "                clave[v] = peso;",
        "                aristaElegida[v] = arista;",
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

        var incluido = new HashSet<int>();
        var clave = new Dictionary<int, double>();
        var aristaElegida = new Dictionary<int, Arista>();
        foreach (var n in grafo.Nodos) clave[n.Id] = double.PositiveInfinity;
        clave[nodoInicio] = 0;

        void AgregarColumna(int uIncluido, HashSet<int> incluidosAntes)
        {
            tabla.Columnas.Add($"Paso {tabla.Columnas.Count + 1}");
            foreach (var n in grafo.Nodos)
            {
                CeldaResultado celda;
                if (incluidosAntes.Contains(n.Id))
                    celda = new CeldaResultado { Texto = "*" };
                else if (clave[n.Id] == double.PositiveInfinity)
                    celda = new CeldaResultado { Texto = "--" };
                else
                {
                    string padreTxt = aristaElegida.TryGetValue(n.Id, out var a)
                        ? Etiqueta(grafo.ObtenerVecino(n.Id, a))
                        : Etiqueta(nodoInicio);
                    celda = new CeldaResultado
                    {
                        Texto = $"({clave[n.Id]:0.##}, {padreTxt})",
                        Destacada = n.Id == uIncluido
                    };
                }
                filaPorNodo[n.Id].Celdas.Add(celda);
            }
        }

        for (int iter = 0; iter < grafo.Nodos.Count; iter++)
        {
            int u = -1;
            double mejor = double.PositiveInfinity;
            foreach (var n in grafo.Nodos)
                if (!incluido.Contains(n.Id) && clave[n.Id] < mejor) { mejor = clave[n.Id]; u = n.Id; }
            if (u == -1) break;

            var incluidosAntes = new HashSet<int>(incluido);
            incluido.Add(u);
            pasos.Add(new PasoAnimacion($"Nodo {u} incluido en el MST").ConNodo(u, EstadoNodo.Visitado).ConLinea(8));
            if (aristaElegida.TryGetValue(u, out var aristaUsada))
                pasos.Add(new PasoAnimacion($"Arista hacia {u} incluida en el MST")
                              .ConArista(aristaUsada.Id, EstadoArista.Incluida)
                              .ConLinea(11));

            foreach (var a in grafo.ObtenerAristasDesde(u))
            {
                int v = grafo.ObtenerVecino(u, a);
                if (!incluido.Contains(v) && a.Peso < clave[v])
                {
                    clave[v] = a.Peso;
                    aristaElegida[v] = a;
                    pasos.Add(new PasoAnimacion($"Nodo {v} ahora se conecta por {u} con costo {a.Peso}")
                                  .ConArista(a.Id, EstadoArista.Explorando)
                                  .ConNodo(v, EstadoNodo.EnCola)
                                  .ConLinea(18));
                }
            }

            AgregarColumna(u, incluidosAntes);
            pasos.Add(new PasoAnimacion($"Columna del paso {tabla.Columnas.Count} completa")
                          .ConRevelarIndice(tabla.Columnas.Count - 1)
                          .ConLinea(22));
        }

        double pesoTotal = aristaElegida.Values.Sum(a => a.Peso);
        string listado = string.Join(", ", aristaElegida.Values.Select(a => $"{Etiqueta(a.OrigenId)}-{Etiqueta(a.DestinoId)}"));
        tabla.Resumen = $"Árbol de expansión mínima — peso total: {pesoTotal:0.##} · aristas: {listado}";
        return new ResultadoAlgoritmo(pasos, tabla);
    }
}
