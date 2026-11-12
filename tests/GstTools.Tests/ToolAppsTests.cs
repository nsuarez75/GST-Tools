using GstTools;

namespace GstTools.Tests;

public sealed class ToolAppsTests : IDisposable
{
    private readonly string _folder = Directory.CreateTempSubdirectory("gsttools-").FullName;
    private static readonly ToolApp Los = ToolApps.All.Single(p => p.Prefix == "GstLosGenerator");

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private string Create(string name, DateTime? date = null)
    {
        var path = Path.Combine(_folder, name);
        File.WriteAllText(path, "");
        File.SetLastWriteTimeUtc(path, date ?? DateTime.UtcNow);
        return path;
    }

    // Los exe de prueba están vacíos: la versión sale del nombre.
    private Executable? Find() => ToolApps.Find(Los, _folder, _ => null);

    [Fact]
    public void Find_NoExe_ReturnsNull()
    {
        Create("HerramientasSICAR-v2.2.0.exe");
        Assert.Null(Find());
    }

    [Fact]
    public void Find_SeveralVersions_ReturnsHighest()
    {
        Create("GstLosGenerator-v0.10.0-win-x64.exe", new DateTime(2026, 1, 1));
        Create("GstLosGenerator-v0.9.5-win-x64.exe", new DateTime(2026, 6, 1));

        var exe = Find();

        Assert.Equal("GstLosGenerator-v0.10.0-win-x64.exe", Path.GetFileName(exe!.FilePath));
        Assert.Equal(new Version(0, 10, 0), exe.Version);
    }

    [Fact]
    public void Find_NoVersionInName_ReturnsNewest()
    {
        Create("GstLosGenerator (1).exe", new DateTime(2026, 1, 1));
        Create("GstLosGenerator.exe", new DateTime(2026, 6, 1));

        var exe = Find();

        Assert.Equal("GstLosGenerator.exe", Path.GetFileName(exe!.FilePath));
        Assert.Null(exe.Version);
    }

    [Fact]
    public void Find_ExeVersion_OverridesName()
    {
        var path = Create("GstLosGenerator.exe");

        var exe = ToolApps.Find(Los, _folder, r => r == path ? new Version(0, 3, 2) : null);

        Assert.Equal(new Version(0, 3, 2), exe!.Version);
    }

    [Fact]
    public void Find_OtherAppSamePrefix_ReturnsNull()
    {
        Create("GstLosGeneratorViejo.exe");
        Assert.Null(Find());
    }

    [Fact]
    public void Find_MissingFolder_ReturnsNull() =>
        Assert.Null(ToolApps.Find(Los, Path.Combine(_folder, "no-existe")));

    [Theory]
    [InlineData("HerramientasSICAR-v2.2.0.exe", "2.2.0")]
    [InlineData("GstFitGenerator-v0.1.4-win-x64.exe", "0.1.4")]
    [InlineData("GstLosGenerator_1.2.exe", "1.2.0")]
    [InlineData("HerramientasSICAR.exe", null)]
    public void VersionFromName_FileName_ReturnsVersion(string fileName, string? expected) =>
        Assert.Equal(expected is null ? null : Version.Parse(expected), ToolApps.VersionFromName(fileName));
}
