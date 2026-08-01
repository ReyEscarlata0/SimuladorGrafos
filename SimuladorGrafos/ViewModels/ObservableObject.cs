using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimuladorGrafos.ViewModels;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Notificar([CallerMemberName] string? nombrePropiedad = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));

    protected bool EstablecerPropiedad<T>(ref T campo, T valor, [CallerMemberName] string? nombrePropiedad = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
        campo = valor;
        Notificar(nombrePropiedad);
        return true;
    }
}
