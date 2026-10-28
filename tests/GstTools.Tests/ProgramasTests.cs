using GstTools;

namespace GstTools.Tests;

public sealed class ProgramasTests : IDisposable
{
    private readonly string _carpeta = Directory.CreateTempSubdirectory("gsttools-").FullName;
    private static readonly Programa Los = Programas.Todos.Single(p => p.Prefijo == "GstLosGenerator");

    public void Dispose() => Directory.Delete(_carpeta, recursive: true);

    private string Crear(string nombre, DateTime? fecha = null)
    {
        var ruta = Path.Combine(_carpeta, nombre);
        File.WriteAllText(ruta, "");
        File.SetLastWriteTimeUtc(ruta, fecha ?? DateTime.UtcNow);
        return ruta;
    }

    // Los exe de prueba están vacíos: la versión sale del nombre.
    private Ejecutable? Buscar() => Programas.Buscar(Los, _carpeta, _ => null);

    [Fact]
    public void Sin_exe_no_encuentra_nada()
    {
        Crear("HerramientasSICAR-v2.2.0.exe");
        Assert.Null(Buscar());
    }

    [Fact]
    public void Coge_la_version_mas_alta_aunque_sea_mas_antigua()
    {
        Crear("GstLosGenerator-v0.10.0-win-x64.exe", new DateTime(2026, 1, 1));
        Crear("GstLosGenerator-v0.9.5-win-x64.exe", new DateTime(2026, 6, 1));

        var exe = Buscar();

        Assert.Equal("GstLosGenerator-v0.10.0-win-x64.exe", Path.GetFileName(exe!.Ruta));
        Assert.Equal(new Version(0, 10, 0), exe.Version);
    }

    [Fact]
    public void Sin_version_en_el_nombre_coge_el_mas_reciente()
    {
        Crear("GstLosGenerator (1).exe", new DateTime(2026, 1, 1));
        Crear("GstLosGenerator.exe", new DateTime(2026, 6, 1));

        var exe = Buscar();

        Assert.Equal("GstLosGenerator.exe", Path.GetFileName(exe!.Ruta));
        Assert.Null(exe.Version);
    }

    [Fact]
    public void La_version_del_exe_manda_sobre_el_nombre()
    {
        var ruta = Crear("GstLosGenerator.exe");

        var exe = Programas.Buscar(Los, _carpeta, r => r == ruta ? new Version(0, 3, 2) : null);

        Assert.Equal(new Version(0, 3, 2), exe!.Version);
    }

    [Fact]
    public void No_confunde_otra_aplicacion_que_empieza_igual()
    {
        Crear("GstLosGeneratorViejo.exe");
        Assert.Null(Buscar());
    }

    [Fact]
    public void Una_carpeta_que_no_existe_no_falla() =>
        Assert.Null(Programas.Buscar(Los, Path.Combine(_carpeta, "no-existe")));

    [Theory]
    [InlineData("HerramientasSICAR-v2.2.0.exe", "2.2.0")]
    [InlineData("GstFitGenerator-v0.1.4-win-x64.exe", "0.1.4")]
    [InlineData("GstLosGenerator_1.2.exe", "1.2.0")]
    [InlineData("HerramientasSICAR.exe", null)]
    public void Version_del_nombre(string archivo, string? esperada) =>
        Assert.Equal(esperada is null ? null : Version.Parse(esperada), Programas.VersionDelNombre(archivo));
}
