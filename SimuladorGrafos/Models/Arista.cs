namespace SimuladorGrafos.Models;

public class Arista
{
    public int Id { get; }
    public int OrigenId { get; }
    public int DestinoId { get; }
    public double Peso { get; set; }
    public bool EsDirigida { get; set; }
    public EstadoArista Estado { get; set; } = EstadoArista.Normal;

    public Arista(int id, int origenId, int destinoId, double peso, bool esDirigida)
    {
        Id = id;
        OrigenId = origenId;
        DestinoId = destinoId;
        Peso = peso;
        EsDirigida = esDirigida;
    }
}
