// Lo que se ve de cada aplicación en el lanzador.
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace GstTools;

public sealed class ToolCard(ToolApp app, Executable? executable, Release? latest)
{
    public ToolApp ToolApp { get; } = app;
    public Executable? Executable { get; } = executable;
    public Release? Latest { get; } = latest;

    public string Name => ToolApp.Name;
    public string Description => L.T(ToolApp.Description);
    public Bitmap Icon { get; } = new(AssetLoader.Open(new Uri("avares://GstTools/Assets/" + app.Icon)));

    public bool Found => Executable is not null;
    public bool Missing => Executable is null;

    public string Status => Executable switch
    {
        null => L.T("No está en esta carpeta"),
        { Version: { } v } => L.T("versión {0}", v.ToString(3)),
        _ => Path.GetFileName(Executable.FilePath),
    };

    public string? FilePath => Executable?.FilePath;

    public bool HasNewer => ToolApps.HasNewer(Executable, Latest);
    public string UpdateNotice => Latest is null ? "" : L.T("Hay una versión nueva: {0}", Latest.Tag);

    // Descargar se ofrece si falta la aplicación o si hay una versión nueva.
    public bool CanDownload => Missing || HasNewer;
    public string DownloadText => Latest is null ? L.T("Descargar la última versión") : L.T("Descargar la versión {0}", Latest.Tag);
    public string DownloadUrl => Latest?.Url ?? ToolApp.DownloadsUrl;
}
