// Lo que se ve de cada aplicación en el lanzador.
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace GstTools;

public sealed class Tarjeta(Programa programa, Ejecutable? ejecutable, Release? ultima)
{
    public Programa Programa { get; } = programa;
    public Ejecutable? Ejecutable { get; } = ejecutable;
    public Release? Ultima { get; } = ultima;

    public string Nombre => Programa.Nombre;
    public string Descripcion => L.T(Programa.Descripcion);
    public Bitmap Icono { get; } = new(AssetLoader.Open(new Uri("avares://GstTools/Assets/" + programa.Icono)));

    public bool Encontrado => Ejecutable is not null;
    public bool NoEncontrado => Ejecutable is null;

    public string Estado => Ejecutable switch
    {
        null => L.T("No está en esta carpeta"),
        { Version: { } v } => L.T("versión {0}", v.ToString(3)),
        _ => Path.GetFileName(Ejecutable.Ruta),
    };

    public string? Ruta => Ejecutable?.Ruta;

    public bool HayNueva => Programas.HayNueva(Ejecutable, Ultima);
    public string AvisoNueva => Ultima is null ? "" : L.T("Hay una versión nueva: {0}", Ultima.Tag);

    // Descargar se ofrece si falta la aplicación o si hay una versión nueva.
    public bool SePuedeDescargar => NoEncontrado || HayNueva;
    public string TextoDescargar => Ultima is null ? L.T("Descargar la última versión") : L.T("Descargar la versión {0}", Ultima.Tag);
    public string UrlDescarga => Ultima?.Url ?? Programa.Descargas;
}
