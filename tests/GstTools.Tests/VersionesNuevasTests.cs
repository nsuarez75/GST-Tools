using GstTools;

namespace GstTools.Tests;

public sealed class VersionesNuevasTests
{
    private static readonly Ejecutable Local = new("GstLosGenerator-v0.3.2-win-x64.exe", new Version(0, 3, 2));

    private static Release Publicada(string tag) => new(tag, Programas.VersionDelTag(tag)!, "https://github.com/x/y/releases/tag/" + tag);

    [Fact]
    public void Lee_la_release_de_github()
    {
        var release = Programas.LeerRelease("""{"tag_name":"v0.3.3","html_url":"https://github.com/nsuarez75/GST-LoS-Generator/releases/tag/v0.3.3","name":"v0.3.3"}""");

        Assert.Equal(new Release("v0.3.3", new Version(0, 3, 3), "https://github.com/nsuarez75/GST-LoS-Generator/releases/tag/v0.3.3"), release);
    }

    [Theory]
    [InlineData("""{"message":"Not Found"}""")]
    [InlineData("""{"tag_name":"latest","html_url":"https://github.com"}""")]
    [InlineData("no es json")]
    public void Una_respuesta_rara_no_es_release(string json) => Assert.Null(Programas.LeerRelease(json));

    [Theory]
    [InlineData("v0.3.3", true)]
    [InlineData("v0.10.0", true)]
    [InlineData("v0.3.2", false)]
    [InlineData("v0.3.1", false)]
    public void Avisa_solo_si_la_publicada_es_mas_nueva(string tag, bool nueva) =>
        Assert.Equal(nueva, Programas.HayNueva(Local, Publicada(tag)));

    [Fact]
    public void Sin_version_local_o_sin_release_no_avisa()
    {
        Assert.False(Programas.HayNueva(Local with { Version = null }, Publicada("v9.0.0")));
        Assert.False(Programas.HayNueva(null, Publicada("v9.0.0")));
        Assert.False(Programas.HayNueva(Local, null));
    }

    [Fact]
    public void La_api_sale_del_repositorio() =>
        Assert.All(Programas.Todos, p => Assert.StartsWith("https://api.github.com/repos/nsuarez75/", p.ApiUltimaRelease));
}
