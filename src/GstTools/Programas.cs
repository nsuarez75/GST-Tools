// Las aplicaciones que abre el lanzador y cómo se encuentra el ejecutable de cada una en la carpeta.
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GstTools;

public sealed record Programa(string Nombre, string Descripcion, string Prefijo, string Icono, string Repositorio)
{
    public string Descargas => Repositorio + "/releases/latest";
}

public sealed record Ejecutable(string Ruta, Version? Version);

public static partial class Programas
{
    public static readonly IReadOnlyList<Programa> Todos =
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
    public static Ejecutable? Buscar(Programa programa, string carpeta, Func<string, Version?>? versionDelArchivo = null)
    {
        if (!Directory.Exists(carpeta))
        {
            return null;
        }
        versionDelArchivo ??= VersionDelArchivo;
        return Directory.EnumerateFiles(carpeta, programa.Prefijo + "*.exe")
            .Where(ruta => EsDelPrograma(Path.GetFileName(ruta), programa.Prefijo))
            .Select(ruta => new Ejecutable(ruta, versionDelArchivo(ruta) ?? VersionDelNombre(Path.GetFileName(ruta))))
            .OrderByDescending(e => e.Version ?? new Version(0, 0))
            .ThenByDescending(e => File.GetLastWriteTimeUtc(e.Ruta))
            .FirstOrDefault();
    }

    // «GstLosGenerator.exe» o «GstLosGenerator-…exe», pero no otra aplicación que empiece igual.
    internal static bool EsDelPrograma(string archivo, string prefijo)
    {
        var resto = Path.GetFileNameWithoutExtension(archivo)[prefijo.Length..];
        return resto.Length == 0 || resto[0] is '-' or '_' or ' ' or '.' or '(';
    }

    internal static Version? VersionDelNombre(string archivo) =>
        VersionEnNombre().Match(archivo) is { Success: true } m ? Normalizar(Version.Parse(m.Groups[1].Value)) : null;

    private static Version? VersionDelArchivo(string ruta)
    {
        var info = FileVersionInfo.GetVersionInfo(ruta);
        // Sin recurso de versión vale 0.0.0.0: entonces manda el nombre.
        return info.FileMajorPart + info.FileMinorPart + info.FileBuildPart == 0
            ? null
            : new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart);
    }

    private static Version Normalizar(Version v) => new(v.Major, v.Minor, Math.Max(v.Build, 0));

    [GeneratedRegex(@"[-_ ]v?(\d+\.\d+(?:\.\d+)?)", RegexOptions.IgnoreCase)]
    private static partial Regex VersionEnNombre();
}
