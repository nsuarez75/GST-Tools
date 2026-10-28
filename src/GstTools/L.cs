// Idioma de la interfaz: el de Windows, castellano o inglés. Los textos se escriben en castellano
// y L.T los traduce; el texto en castellano es la clave, así que uno sin traducción se muestra tal cual.
using System.Globalization;

namespace GstTools;

public static class L
{
    public const string Spanish = "es";
    public const string English = "en";

    public static string Language { get; set; } = Spanish;

    public static string SystemLanguage() =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == Spanish ? Spanish : English;

    public static string T(string text) => Language == English && En.TryGetValue(text, out var translated) ? translated : text;

    // Textos con huecos {0}, {1}...: se traduce el patrón y después se rellenan.
    public static string T(string format, params object?[] args) => string.Format(CultureInfo.InvariantCulture, T(format), args);

    internal static IReadOnlyDictionary<string, string> EnglishTexts => En;

    private static readonly Dictionary<string, string> En = new()
    {
        ["Lanzador de las aplicaciones para proyectos SICAR@GST"] = "Launcher for the SICAR@GST project applications",
        ["Genera el FB de devices y sus UDT a partir de la LoS."] = "Generates the devices FB and its UDTs from the LoS.",
        ["Genera el FIT y los bloques de trazabilidad del PLC a partir de las features de cada referencia."] =
            "Generates the FIT and the PLC traceability blocks from the features of each reference.",
        ["Tareas sobre un proyecto SICAR@GST abierto en TIA Portal: importar y exportar, mensajes, comentarios, HMI…"] =
            "Tasks on a SICAR@GST project open in TIA Portal: import and export, messages, comments, HMI…",
        ["Abrir"] = "Open",
        ["Descargar la última versión"] = "Download the latest version",
        ["Descargar la versión {0}"] = "Download version {0}",
        ["Hay una versión nueva: {0}"] = "New version available: {0}",
        ["versión {0}"] = "version {0}",
        ["No está en esta carpeta"] = "Not in this folder",
        ["Se buscan en la carpeta del lanzador: {0}"] = "Looked for in the launcher folder: {0}",
        ["Abrir la carpeta"] = "Open the folder",
        ["Abriendo {0}…"] = "Opening {0}…",
        ["No se pudo abrir {0}: {1}"] = "Could not open {0}: {1}",
    };
}
