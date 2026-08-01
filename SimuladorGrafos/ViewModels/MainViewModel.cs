using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SimuladorGrafos.Algoritmos;
using SimuladorGrafos.Models;
using SimuladorGrafos.Services;
using SimuladorGrafos.Views;

namespace SimuladorGrafos.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly Grafo _grafo = new();
    private readonly ReproductorAnimacion _reproductor = new();
    private NodoVista? _origenAristaPendiente;

    public ObservableCollection<NodoVista> Nodos { get; } = new();
    public ObservableCollection<AristaVista> Aristas { get; } = new();

    public List<IAlgoritmoGrafo> Algoritmos { get; } = new()
    {
        new AlgoritmoBFS(),
        new AlgoritmoDFS(),
        new AlgoritmoDijkstra(),
        new AlgoritmoBellmanFord(),
        new AlgoritmoPrim(),
        new AlgoritmoKruskal(),
    };

    public string[] NombresEjemplos => GrafosEjemplo.Nombres;

    private IAlgoritmoGrafo _algoritmoSeleccionado;
    public IAlgoritmoGrafo AlgoritmoSeleccionado
    {
        get => _algoritmoSeleccionado;
        set
        {
            if (EstablecerPropiedad(ref _algoritmoSeleccionado, value))
                CargarLineasCodigo();
        }
    }

    public ObservableCollection<LineaCodigoVista> LineasCodigo { get; } = new();

    private int _lineaActualIndice = -1;
    public int LineaActualIndice
    {
        get => _lineaActualIndice;
        set => EstablecerPropiedad(ref _lineaActualIndice, value);
    }

    private NodoVista? _nodoInicioSeleccionado;
    public NodoVista? NodoInicioSeleccionado
    {
        get => _nodoInicioSeleccionado;
        set => EstablecerPropiedad(ref _nodoInicioSeleccionado, value);
    }

    private string _modoEdicion = "Ninguno";
    public string ModoEdicion
    {
        get => _modoEdicion;
        set => EstablecerPropiedad(ref _modoEdicion, value);
    }

    private string _log = "Elegí 'Agregar Nodo' para empezar a dibujar, o cargá un grafo de ejemplo.";
    public string Log
    {
        get => _log;
        set => EstablecerPropiedad(ref _log, value);
    }

    private double _velocidad = 1.0;
    public double Velocidad
    {
        get => _velocidad;
        set
        {
            if (EstablecerPropiedad(ref _velocidad, value))
                _reproductor.Velocidad = value;
        }
    }

    private TablaResultado? _tablaResultado;
    public TablaResultado? TablaResultado
    {
        get => _tablaResultado;
        set => EstablecerPropiedad(ref _tablaResultado, value);
    }

    private int _indiceRevelado = -1;
    public int IndiceRevelado
    {
        get => _indiceRevelado;
        set => EstablecerPropiedad(ref _indiceRevelado, value);
    }

    public ICommand ActivarModoNodoCommand { get; }
    public ICommand ActivarModoAristaCommand { get; }
    public ICommand ClicEnCanvasCommand { get; }
    public ICommand ClicEnNodoCommand { get; }
    public ICommand CargarEjemploCommand { get; }
    public ICommand NuevoGrafoCommand { get; }
    public ICommand PlayCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StepCommand { get; }
    public ICommand ResetCommand { get; }

    public MainViewModel()
    {
        _algoritmoSeleccionado = Algoritmos[0];
        CargarLineasCodigo();

        _reproductor.PasoAplicado += paso =>
        {
            Log = paso.Descripcion;
            if (paso.RevelarIndice.HasValue) IndiceRevelado = paso.RevelarIndice.Value;
            ResaltarLinea(paso.LineaResaltada);
        };
        _reproductor.Terminado += () => Log = "Animación terminada.";

        ActivarModoNodoCommand = new RelayCommand(_ => ModoEdicion = "Nodo");
        ActivarModoAristaCommand = new RelayCommand(_ =>
        {
            ModoEdicion = "Arista";
            _origenAristaPendiente = null;
        });
        ClicEnCanvasCommand = new RelayCommand(p => ManejarClicCanvas((Point)p!));
        ClicEnNodoCommand = new RelayCommand(p => ManejarClicNodo((NodoVista)p!));
        CargarEjemploCommand = new RelayCommand(p => CargarEjemplo(int.Parse((string)p!)));
        NuevoGrafoCommand = new RelayCommand(_ => NuevoGrafo());
        PlayCommand = new RelayCommand(_ => Reproducir());
        PauseCommand = new RelayCommand(_ => _reproductor.Pausar());
        StepCommand = new RelayCommand(_ => Reproducir(soloUnPaso: true));
        ResetCommand = new RelayCommand(_ => ReiniciarAnimacion());
    }

    private void ManejarClicCanvas(Point punto)
    {
        if (ModoEdicion != "Nodo") return;

        var nodoModelo = _grafo.AgregarNodo(punto.X, punto.Y);
        Nodos.Add(new NodoVista(nodoModelo));
        Log = $"Nodo {nodoModelo.Id} agregado.";
    }

    private void ManejarClicNodo(NodoVista nodo)
    {
        if (ModoEdicion != "Arista") return;

        if (_origenAristaPendiente == null)
        {
            _origenAristaPendiente = nodo;
            Log = $"Arista: hacé clic en el nodo destino (origen = {nodo.Etiqueta}).";
            return;
        }

        var dialogo = new VentanaPesoArista { Owner = Application.Current.MainWindow };
        if (dialogo.ShowDialog() == true)
        {
            var aristaModelo = _grafo.AgregarArista(_origenAristaPendiente.Id, nodo.Id, dialogo.Peso, dialogo.EsDirigida);
            Aristas.Add(new AristaVista(aristaModelo, _origenAristaPendiente, nodo));
            Log = $"Arista {_origenAristaPendiente.Etiqueta}-{nodo.Etiqueta} (peso {dialogo.Peso}) agregada.";
        }

        _origenAristaPendiente = null;
    }

    private void CargarEjemplo(int indice)
    {
        NuevoGrafo();
        GrafosEjemplo.Cargar(indice, _grafo);
        SincronizarVistasDesdeModelo();
        Log = $"{GrafosEjemplo.Nombres[indice]} cargado.";
    }

    private void SincronizarVistasDesdeModelo()
    {
        Nodos.Clear();
        Aristas.Clear();

        var mapa = new Dictionary<int, NodoVista>();
        foreach (var n in _grafo.Nodos)
        {
            var nv = new NodoVista(n);
            mapa[n.Id] = nv;
            Nodos.Add(nv);
        }
        foreach (var a in _grafo.Aristas)
            Aristas.Add(new AristaVista(a, mapa[a.OrigenId], mapa[a.DestinoId]));
    }

    private void NuevoGrafo()
    {
        _reproductor.Detener();
        _grafo.Limpiar();
        Nodos.Clear();
        Aristas.Clear();
        NodoInicioSeleccionado = null;
        _origenAristaPendiente = null;
        TablaResultado = null;
        IndiceRevelado = -1;
        ResaltarLinea(null);
        Log = "Grafo nuevo. Elegí 'Agregar Nodo' o 'Agregar Arista'.";
    }

    private void Reproducir(bool soloUnPaso = false)
    {
        if (!_reproductor.TienePasos)
        {
            if (Nodos.Count == 0)
            {
                Log = "Dibujá un grafo o cargá un ejemplo primero.";
                return;
            }
            if (AlgoritmoSeleccionado.RequiereNodoInicio && NodoInicioSeleccionado == null)
            {
                Log = "Elegí un nodo de inicio primero.";
                return;
            }

            _grafo.ReiniciarEstados();
            SincronizarEstadosVisuales();
            TablaResultado = null;
            IndiceRevelado = -1;
            ResaltarLinea(null);

            int inicio = NodoInicioSeleccionado?.Id ?? _grafo.Nodos[0].Id;
            ResultadoAlgoritmo resultado;
            try
            {
                resultado = AlgoritmoSeleccionado.Ejecutar(_grafo, inicio);
            }
            catch (NotImplementedException)
            {
                // Esperado mientras el algoritmo todavía es un stub: confirma que
                // el cableado UI -> algoritmo -> animación funciona; falta la lógica.
                Log = $"'{AlgoritmoSeleccionado.Nombre}' todavía no está implementado (ver TODO en Algoritmos/).";
                return;
            }
            TablaResultado = resultado.Tabla;
            _reproductor.Cargar(resultado.Pasos, Nodos, Aristas);
        }

        if (soloUnPaso) _reproductor.Paso();
        else _reproductor.Reproducir();
    }

    private void ReiniciarAnimacion()
    {
        _reproductor.Detener();
        _grafo.ReiniciarEstados();
        SincronizarEstadosVisuales();
        TablaResultado = null;
        IndiceRevelado = -1;
        ResaltarLinea(null);
        Log = "Animación reiniciada.";
    }

    private void SincronizarEstadosVisuales()
    {
        foreach (var nv in Nodos) nv.Estado = EstadoNodo.SinVisitar;
        foreach (var av in Aristas) av.Estado = EstadoArista.Normal;
    }

    private void CargarLineasCodigo()
    {
        LineasCodigo.Clear();
        var codigo = AlgoritmoSeleccionado.Codigo;
        for (int i = 0; i < codigo.Length; i++)
            LineasCodigo.Add(new LineaCodigoVista(i, codigo[i]));
        LineaActualIndice = -1;
    }

    private void ResaltarLinea(int? indice)
    {
        foreach (var linea in LineasCodigo) linea.EsActual = linea.Indice == indice;
        LineaActualIndice = indice ?? -1;
    }
}
