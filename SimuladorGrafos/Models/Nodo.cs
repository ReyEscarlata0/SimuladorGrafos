namespace SimuladorGrafos.Models;

public class Nodo
{
    public int Id { get; }
    public string Etiqueta { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public EstadoNodo Estado { get; set; } = EstadoNodo.SinVisitar;

    public Nodo(int id, string etiqueta, double x, double y)
    {
        Id = id;
        Etiqueta = etiqueta;
        X = x;
        Y = y;
    }
}
