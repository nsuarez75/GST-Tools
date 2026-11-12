using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace GstTools;

public partial class MainWindow : Window
{
    // Las aplicaciones se buscan junto al lanzador.
    private static readonly string Folder = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;

    // Última release de cada aplicación en GitHub; se consulta una vez al abrir.
    private readonly Dictionary<ToolApp, Release?> _latest = [];

    public MainWindow()
    {
        InitializeComponent();
        Find();
        Opened += async (_, _) => await CheckForUpdates();
        Notice.Text = L.T("Se buscan en la carpeta del lanzador: {0}", Folder);
        // Al volver a la ventana se busca otra vez: así se ve un exe que se acaba de copiar o actualizar.
        Activated += (_, _) => Find();
    }

    private void Find() =>
        AppList.ItemsSource = ToolApps.All
            .Select(p => new ToolCard(p, ToolApps.Find(p, Folder), _latest.GetValueOrDefault(p)))
            .ToList();

    private async Task CheckForUpdates()
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        // GitHub rechaza las peticiones sin User-Agent.
        http.DefaultRequestHeaders.UserAgent.ParseAdd("GstTools");
        var latest = await Task.WhenAll(ToolApps.All.Select(p => ToolApps.LatestRelease(http, p)));
        for (var i = 0; i < latest.Length; i++)
        {
            _latest[ToolApps.All[i]] = latest[i];
        }
        Find();
    }

    private void OpenApp(object? sender, RoutedEventArgs e)
    {
        if ((sender as Control)?.DataContext is not ToolCard { FilePath: { } path } card)
        {
            return;
        }
        try
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true, WorkingDirectory = Folder });
            Notice.Text = L.T("Abriendo {0}…", card.Name);
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or IOException)
        {
            Notice.Text = L.T("No se pudo abrir {0}: {1}", card.Name, ex.Message);
        }
    }

    private void Download(object? sender, RoutedEventArgs e)
    {
        if ((sender as Control)?.DataContext is ToolCard card)
        {
            Launch(card.DownloadUrl);
        }
    }

    private async void ShowAbout(object? sender, RoutedEventArgs e) => await new AboutWindow().ShowDialog(this);

    private void OpenFolder(object? sender, RoutedEventArgs e) => Launch(Folder);

    private void Launch(string target)
    {
        try
        {
            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or IOException)
        {
            Notice.Text = L.T("No se pudo abrir {0}: {1}", target, ex.Message);
        }
    }
}
