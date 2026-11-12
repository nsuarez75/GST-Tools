// Las aplicaciones que abre el lanzador y cómo se encuentra el ejecutable de cada una en la carpeta.
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GstTools;

public sealed record ToolApp(string Name, string Description, string Prefix, string Icon, string Repository)
{
    public string DownloadsUrl => Repository + "/releases/latest";

    // API de GitHub de la última release: owner/repo sacados de la URL del repositorio.
    public string LatestReleaseApi => "https://api.github.com/repos/" + Repository["https://github.com/".Length..] + "/releases/latest";
}

public sealed record Executable(string FilePath, Version? Version);

public sealed record Release(string Tag, Version Version, string Url);

public static partial class ToolApps
{
    public static readonly IReadOnlyList<ToolApp> All =
    [
        new("HerramientasSICAR",
            "Tareas sobre un proyecto SICAR@GST abierto en TIA Portal: importar y exportar, mensajes, comentarios, HMI…",
            "HerramientasSICAR", "alomatek.png", "https://github.com/nsuarez75/HerramientasSicar"),
        new("GST LoS Generator",
            "Genera el FB de devices y sus UDT a partir de la LoS.",
            "GstLosGenerator", "gst-los-generator.png", "https://github.com/nsuarez75/GST-LoS-Generator"),
        new("GST FIT Generator",
            "Genera el FIT y los bloques de trazabilidad del PLC a partir de las features de cada referencia.",
            "GstFitGenerator", "gst-fit-generator.png", "https://github.com/nsuarez75/GST-FIT-Generator"),
    ];

    /// <summary>
    /// El ejecutable de la aplicación en la carpeta: el que empieza por su prefijo, y si hay varios, el de versión
    /// más alta (a igualdad, el más reciente). La versión sale del propio exe o, si no la trae, del nombre (-v1.2.3).
    /// </summary>
    public static Executable? Find(ToolApp app, string folder, Func<string, Version?>? fileVersion = null)
    {
        if (!Directory.Exists(folder))
        {
            return null;
        }
        fileVersion ??= FileVersion;
        return Directory.EnumerateFiles(folder, app.Prefix + "*.exe")
            .Where(path => BelongsTo(Path.GetFileName(path), app.Prefix))
            .Select(path => new Executable(path, fileVersion(path) ?? VersionFromName(Path.GetFileName(path))))
            .OrderByDescending(e => e.Version ?? new Version(0, 0))
            .ThenByDescending(e => File.GetLastWriteTimeUtc(e.FilePath))
            .FirstOrDefault();
    }

    /// <summary>Última release publicada en GitHub, o null si no se puede saber (sin red, sin releases...).</summary>
    public static async Task<Release?> LatestRelease(HttpClient http, ToolApp app, CancellationToken cancel = default)
    {
        try
        {
            using var response = await http.GetAsync(app.LatestReleaseApi, cancel);
            return response.IsSuccessStatusCode ? ReadRelease(await response.Content.ReadAsStringAsync(cancel)) : null;
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return null;
        }
    }

    internal static Release? ReadRelease(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var tag = root.TryGetProperty("tag_name", out var t) ? t.GetString() : null;
            var url = root.TryGetProperty("html_url", out var u) ? u.GetString() : null;
            return tag is not null && url is not null && VersionFromTag(tag) is { } version ? new Release(tag, version, url) : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    internal static Version? VersionFromTag(string tag) =>
        Version.TryParse(tag.TrimStart('v', 'V'), out var v) ? Normalize(v) : null;

    /// <summary>Hay una versión publicada más nueva que la de la carpeta. Si no se sabe la de la carpeta, no se avisa.</summary>
    public static bool HasNewer(Executable? executable, Release? latest) =>
        executable?.Version is { } current && latest is not null && latest.Version > current;

    // «GstLosGenerator.exe» o «GstLosGenerator-…exe», pero no otra aplicación que empiece igual.
    internal static bool BelongsTo(string fileName, string prefix)
    {
        var rest = Path.GetFileNameWithoutExtension(fileName)[prefix.Length..];
        return rest.Length == 0 || rest[0] is '-' or '_' or ' ' or '.' or '(';
    }

    internal static Version? VersionFromName(string fileName) =>
        VersionInName().Match(fileName) is { Success: true } m ? Normalize(Version.Parse(m.Groups[1].Value)) : null;

    private static Version? FileVersion(string path)
    {
        var info = FileVersionInfo.GetVersionInfo(path);
        // Sin recurso de versión vale 0.0.0.0: entonces manda el nombre.
        return info.FileMajorPart + info.FileMinorPart + info.FileBuildPart == 0
            ? null
            : new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart);
    }

    private static Version Normalize(Version v) => new(v.Major, v.Minor, Math.Max(v.Build, 0));

    [GeneratedRegex(@"[-_ ]v?(\d+\.\d+(?:\.\d+)?)", RegexOptions.IgnoreCase)]
    private static partial Regex VersionInName();
}
