using System.Windows.Input;

namespace SimuladorGrafos.ViewModels;

/// Implementación mínima de ICommand para poder enlazar Buttons del XAML a
/// métodos del ViewModel sin depender de ningún paquete NuGet de MVVM.
public class RelayCommand : ICommand
{
    private readonly Action<object?> _ejecutar;
    private readonly Func<object?, bool>? _puedeEjecutar;

    public RelayCommand(Action<object?> ejecutar, Func<object?, bool>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar;
        _puedeEjecutar = puedeEjecutar;
    }

    public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _ejecutar(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
