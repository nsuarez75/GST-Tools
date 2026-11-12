using GstTools;

namespace GstTools.Tests;

public sealed class TranslationTests
{
    // Toda descripción de aplicación tiene que estar traducida: si no, en inglés saldría en castellano.
    [Fact]
    public void Description_AllApps_HaveEnglishTranslation() =>
        Assert.All(ToolApps.All, p => Assert.True(L.EnglishTexts.ContainsKey(p.Description), p.Name));
}
