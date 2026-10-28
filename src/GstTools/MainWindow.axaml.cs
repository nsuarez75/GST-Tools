using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace GstTools;

public partial class MainWindow : Window
{
    // Las aplicaciones se buscan junto al lanzador.
    private static readonly string Carpeta = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;

    // Última release de cada aplicación en GitHub; se consulta una vez al abrir.
    private readonly Dictionary<Programa, Release?> _ultimas = [];

    public MainWindow()
    {
        InitializeComponent();
        Buscar();
        Opened += async (_, _) => await BuscarVersionesNuevas();
        Aviso.Text = L.T("Se buscan en la carpeta del lanzador: {0}", Carpeta);
        // Al volver a la ventana se busca otra vez: así se ve un exe que se acaba de copiar o actualizar.
        Activated += (_, _) => Buscar();
    }

    private void Buscar() =>
        Lista.ItemsSource = Programas.Todos
            .Select(p => new Tarjeta(p, Programas.Buscar(p, Carpeta), _ultimas.GetValueOrDefault(p)))
            .ToList();

    private async Task BuscarVersionesNuevas()
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        // GitHub rechaza las peticiones sin User-Agent.
        http.DefaultRequestHeaders.UserAgent.ParseAdd("GstTools");
        var ultimas = await Task.WhenAll(Programas.Todos.Select(p => Programas.UltimaRelease(http, p)));
        for (var i = 0; i < ultimas.Length; i++)
        {
            _ultimas[Programas.Todos[i]] = ultimas[i];
        }
        Buscar();
    }

    private void Abrir(object? sender, RoutedEventArgs e)
    {
        if ((sender as Control)?.DataContext is not Tarjeta { Ruta: { } ruta } tarjeta)
        {
            return;
        }
        try
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true, WorkingDirectory = Carpeta });
            Aviso.Text = L.T("Abriendo {0}…", tarjeta.Nombre);
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or IOException)
        {
            Aviso.Text = L.T("No se pudo abrir {0}: {1}", tarjeta.Nombre, ex.Message);
        }
    }

    private void Descargar(object? sender, RoutedEventArgs e)
    {
        if ((sender as Control)?.DataContext is Tarjeta tarjeta)
        {
            Lanzar(tarjeta.UrlDescarga);
        }
    }

    private void AbrirCarpeta(object? sender, RoutedEventArgs e) => Lanzar(Carpeta);

    private void Lanzar(string destino)
    {
        try
        {
            Process.Start(new ProcessStartInfo(destino) { UseShellExecute = true });
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or IOException)
        {
            Aviso.Text = L.T("No se pudo abrir {0}: {1}", destino, ex.Message);
        }
    }
}
