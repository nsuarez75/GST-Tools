// Lo que se ve de cada aplicación en el lanzador.
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace GstTools;

public sealed class Tarjeta(Programa programa, Ejecutable? ejecutable)
{
    public Programa Programa { get; } = programa;
    public Ejecutable? Ejecutable { get; } = ejecutable;

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
}
