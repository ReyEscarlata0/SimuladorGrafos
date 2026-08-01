using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SimuladorGrafos.Algoritmos;
using SimuladorGrafos.Services;
using SimuladorGrafos.ViewModels;

namespace SimuladorGrafos;

public partial class MainWindow : Window
{
    private static readonly Style ColumnaCentradaStyle = new(typeof(TextBlock))
    {
        Setters =
        {
            new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center),
            new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
        }
    };

    private static readonly Brush CeldaDestacadaFondo = new SolidColorBrush(Color.FromRgb(0x1E, 0x4D, 0x3E));
    private static readonly Brush CeldaDestacadaTexto = new SolidColorBrush(Color.FromRgb(0x6F, 0xF0, 0xCF));

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    private NodoVista? _nodoArrastrando;
    private Point _offsetArrastre;

    public MainWindow()
    {
        InitializeComponent();
        TemaVentana.AplicarModoOscuro(this);
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainViewModel.TablaResultado) or nameof(MainViewModel.IndiceRevelado))
            ActualizarTablaResultado();

        if (e.PropertyName == nameof(MainViewModel.LineaActualIndice))
            Dispatcher.BeginInvoke(DispatcherPriority.Background, DesplazarACodigoActual);
    }

    private void DesplazarACodigoActual()
    {
        int indice = ViewModel.LineaActualIndice;
        if (indice < 0 || indice >= ListaCodigo.Items.Count) return;

        if (ListaCodigo.ItemContainerGenerator.ContainerFromIndex(indice) is FrameworkElement contenedor)
            contenedor.BringIntoView();
    }

    private void ActualizarTablaResultado()
    {
        var tabla = ViewModel.TablaResultado;
        GridResultado.Columns.Clear();

        if (tabla == null)
        {
            GridResultado.ItemsSource = null;
            return;
        }

        GridResultado.Columns.Add(new DataGridTextColumn
        {
            Header = tabla.TituloPrimeraColumna,
            Binding = new Binding(nameof(FilaResultado.Etiqueta))
        });

        if (tabla.Orientacion == OrientacionTabla.PorColumnas)
        {
            int ultimaColumnaVisible = Math.Min(ViewModel.IndiceRevelado, tabla.Columnas.Count - 1);
            for (int c = 0; c <= ultimaColumnaVisible; c++)
                GridResultado.Columns.Add(CrearColumnaDeCelda(tabla.Columnas[c], c));

            GridResultado.ItemsSource = tabla.Filas;
        }
        else
        {
            for (int c = 0; c < tabla.Columnas.Count; c++)
                GridResultado.Columns.Add(CrearColumnaDeCelda(tabla.Columnas[c], c));

            int filasVisibles = Math.Max(0, ViewModel.IndiceRevelado + 1);
            GridResultado.ItemsSource = tabla.Filas.Take(filasVisibles).ToList();
        }
    }

    private DataGridTextColumn CrearColumnaDeCelda(string encabezado, int indiceColumna)
    {
        return new DataGridTextColumn
        {
            Header = encabezado,
            Binding = new Binding($"Celdas[{indiceColumna}].Texto"),
            ElementStyle = ColumnaCentradaStyle,
            CellStyle = CrearEstiloCeldaDestacada(indiceColumna)
        };
    }

    private Style CrearEstiloCeldaDestacada(int indiceColumna)
    {
        var estiloBase = (Style)FindResource(typeof(DataGridCell));
        var estilo = new Style(typeof(DataGridCell), estiloBase);
        var trigger = new DataTrigger
        {
            Binding = new Binding($"Celdas[{indiceColumna}].Destacada"),
            Value = true
        };
        trigger.Setters.Add(new Setter(DataGridCell.BackgroundProperty, CeldaDestacadaFondo));
        trigger.Setters.Add(new Setter(DataGridCell.ForegroundProperty, CeldaDestacadaTexto));
        trigger.Setters.Add(new Setter(DataGridCell.FontWeightProperty, FontWeights.Bold));
        estilo.Triggers.Add(trigger);
        return estilo;
    }

    private void LienzoGrafo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Si el clic fue sobre un nodo, Nodo_MouseLeftButtonDown ya lo manejó
        // (e.Handled = true) y no llega hasta acá.
        var punto = e.GetPosition(LienzoGrafo);
        ViewModel.ClicEnCanvasCommand.Execute(punto);
    }

    private void Nodo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        var nodoVista = (NodoVista)((FrameworkElement)sender).DataContext;

        // En modo "Arista" el clic en un nodo elige origen/destino; en
        // cualquier otro caso (o sin modo activo), el clic arrastra el nodo.
        if (ViewModel.ModoEdicion == "Arista")
        {
            ViewModel.ClicEnNodoCommand.Execute(nodoVista);
            return;
        }

        _nodoArrastrando = nodoVista;
        var posicionMouse = e.GetPosition(LienzoGrafo);
        _offsetArrastre = new Point(posicionMouse.X - nodoVista.Modelo.X, posicionMouse.Y - nodoVista.Modelo.Y);
        ((UIElement)sender).CaptureMouse();
    }

    private void Nodo_MouseMove(object sender, MouseEventArgs e)
    {
        if (_nodoArrastrando == null || e.LeftButton != MouseButtonState.Pressed) return;

        var posicionMouse = e.GetPosition(LienzoGrafo);
        _nodoArrastrando.MoverA(posicionMouse.X - _offsetArrastre.X, posicionMouse.Y - _offsetArrastre.Y);
    }

    private void Nodo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_nodoArrastrando == null) return;

        ((UIElement)sender).ReleaseMouseCapture();
        _nodoArrastrando = null;
    }
}
