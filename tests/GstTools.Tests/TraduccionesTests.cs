using GstTools;

namespace GstTools.Tests;

public sealed class TraduccionesTests
{
    // Toda descripción de aplicación tiene que estar traducida: si no, en inglés saldría en castellano.
    [Fact]
    public void Las_descripciones_estan_traducidas() =>
        Assert.All(Programas.Todos, p => Assert.True(L.EnglishTexts.ContainsKey(p.Descripcion), p.Nombre));
}
