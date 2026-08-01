using System.Collections.ObjectModel;
using System.Windows.Threading;
using SimuladorGrafos.Algoritmos;
using SimuladorGrafos.ViewModels;

namespace SimuladorGrafos.Services;

/// <summary>
/// Reproduce una lista de PasoAnimacion aplicando los cambios de estado a las
/// NodoVista/AristaVista correspondientes, a intervalos regulares
/// (DispatcherTimer). No conoce nada de algoritmos: solo sabe "aplicar un
/// paso a la UI", uno a la vez, en Play, Step o Reset.
/// </summary>
public class ReproductorAnimacion
{
    private readonly DispatcherTimer _timer = new();
    private List<PasoAnimacion> _pasos = new();
    private ObservableCollection<NodoVista> _nodos = new();
    private ObservableCollection<AristaVista> _aristas = new();
    private int _indice;

    public double Velocidad { get; set; } = 1.0;

    public bool TienePasos => _pasos.Count > 0;

    public event Action<PasoAnimacion>? PasoAplicado;
    public event Action? Terminado;

    public ReproductorAnimacion()
    {
        _timer.Tick += (_, _) => Paso();
    }

    public void Cargar(List<PasoAnimacion> pasos, ObservableCollection<NodoVista> nodos, ObservableCollection<AristaVista> aristas)
    {
        Detener();
        _pasos = pasos;
        _nodos = nodos;
        _aristas = aristas;
        _indice = 0;
    }

    public void Reproducir()
    {
        _timer.Interval = TimeSpan.FromMilliseconds(1000 / Math.Max(0.1, Velocidad));
        _timer.Start();
    }

    public void Pausar() => _timer.Stop();

    public void Paso()
    {
        if (_indice >= _pasos.Count)
        {
            _timer.Stop();
            Terminado?.Invoke();
            return;
        }

        var paso = _pasos[_indice];
        foreach (var (nodoId, estado) in paso.CambiosNodo)
        {
            var nodo = _nodos.FirstOrDefault(n => n.Id == nodoId);
            if (nodo != null) nodo.Estado = estado;
        }
        foreach (var (aristaId, estado) in paso.CambiosArista)
        {
            var arista = _aristas.FirstOrDefault(a => a.Id == aristaId);
            if (arista != null) arista.Estado = estado;
        }

        PasoAplicado?.Invoke(paso);
        _indice++;

        if (_indice >= _pasos.Count)
        {
            _timer.Stop();
            Terminado?.Invoke();
        }
    }

    public void Detener()
    {
        _timer.Stop();
        _pasos = new List<PasoAnimacion>();
        _indice = 0;
    }
}
