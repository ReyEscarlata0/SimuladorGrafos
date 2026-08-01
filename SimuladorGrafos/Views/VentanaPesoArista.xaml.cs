using System.Globalization;
using System.Windows;
using SimuladorGrafos.Services;

namespace SimuladorGrafos.Views;

public partial class VentanaPesoArista : Window
{
    public double Peso { get; private set; }
    public bool EsDirigida { get; private set; }

    public VentanaPesoArista()
    {
        InitializeComponent();
        TemaVentana.AplicarModoOscuro(this);
    }

    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(TxtPeso.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var peso))
        {
            MessageBox.Show(this, "Ingresá un número válido para el peso (ej. 5 o 5.5).", "Peso inválido",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Peso = peso;
        EsDirigida = ChkDirigida.IsChecked == true;
        DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
