using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SimuladorGrafos.Services;

/// Aplica la barra de título oscura de Windows 11 (DWM) a una ventana WPF.
public static class TemaVentana
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int pvAttribute, int cbAttribute);

    private const int DwmwaUseImmersiveDarkMode = 20;

    public static void AplicarModoOscuro(Window ventana)
    {
        void Aplicar()
        {
            try
            {
                var hwnd = new WindowInteropHelper(ventana).Handle;
                int valor = 1;
                DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkMode, ref valor, sizeof(int));
            }
            catch (DllNotFoundException)
            {
                // Sistema sin soporte DWM inmersivo: se ignora, la ventana sigue funcionando con el tema por defecto.
            }
        }

        if (ventana.IsLoaded)
        {
            Aplicar();
        }
        else
        {
            ventana.SourceInitialized += (_, _) => Aplicar();
        }
    }
}
