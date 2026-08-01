using SimuladorGrafos.Models;

namespace SimuladorGrafos.Services;

/// <summary>
/// Grafos pre-armados para poder probar los algoritmos sin tener que dibujar
/// un grafo a mano. Son solo datos (posiciones + aristas), sin lógica de
/// ningún algoritmo.
/// </summary>
public static class GrafosEjemplo
{
    public static readonly string[] Nombres =
    {
        "Ejemplo 1: grafo dirigido (8 nodos)",
        "Ejemplo 2: grafo no dirigido (6 nodos)"
    };

    private static readonly (double X, double Y)[] PosicionesEjemplo1 =
    {
        (420, 60), (240, 180), (600, 180), (110, 340), (720, 340), (270, 480), (560, 480), (420, 560)
    };

    private static readonly (int Origen, int Destino, double Peso)[] AristasEjemplo1 =
    {
        (0, 1, 8), (0, 2, 3), (1, 2, 7), (1, 3, 5), (2, 4, 7),
        (3, 5, 6), (3, 7, 5), (4, 6, 8), (5, 7, 5), (6, 7, 9)
    };

    private static readonly (double X, double Y)[] PosicionesEjemplo2 =
    {
        (150, 100), (400, 80), (650, 100), (150, 320), (400, 350), (650, 320)
    };

    private static readonly (int Origen, int Destino, double Peso)[] AristasEjemplo2 =
    {
        (0, 1, 4), (0, 3, 2), (1, 2, 5), (1, 4, 1), (2, 5, 3), (3, 4, 6), (4, 5, 2)
    };

    public static void Cargar(int indice, Grafo grafo)
    {
        switch (indice)
        {
            case 0:
                CargarGrafo(grafo, PosicionesEjemplo1, AristasEjemplo1, esDirigida: true);
                break;
            case 1:
                CargarGrafo(grafo, PosicionesEjemplo2, AristasEjemplo2, esDirigida: false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(indice));
        }
    }

    private static void CargarGrafo(
        Grafo grafo,
        (double X, double Y)[] posiciones,
        (int Origen, int Destino, double Peso)[] aristas,
        bool esDirigida)
    {
        foreach (var (x, y) in posiciones)
            grafo.AgregarNodo(x, y);

        foreach (var (origen, destino, peso) in aristas)
            grafo.AgregarArista(origen, destino, peso, esDirigida);
    }
}
