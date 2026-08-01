using SimuladorGrafos.Models;

namespace SimuladorGrafos.Algoritmos;

public interface IAlgoritmoGrafo
{
    string Nombre { get; }

    /// Si es false, el algoritmo no necesita nodo de inicio (ej. Kruskal).
    bool RequiereNodoInicio { get; }

    /// Pseudocódigo del algoritmo, una línea por elemento, mostrado en el
    /// panel "Código en vivo" y resaltado según PasoAnimacion.LineaResaltada.
    string[] Codigo { get; }

    ResultadoAlgoritmo Ejecutar(Grafo grafo, int nodoInicio);
}
