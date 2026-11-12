using GstTools;

namespace GstTools.Tests;

public sealed class AboutTests
{
    [Fact]
    public void Text_Spanish_ShowsNameVersionAndAuthor()
    {
        L.Language = L.Spanish;
        Assert.Equal($"GST Tools, versión {AboutWindow.CurrentVersion.ToString(3)}.\nCreado por Ignacio Suarez (Alomatek).", AboutWindow.Text());
    }

    [Fact]
    public void Text_English_IsTranslated()
    {
        L.Language = L.English;
        try
        {
            Assert.StartsWith("GST Tools, version ", AboutWindow.Text());
        }
        finally
        {
            L.Language = L.Spanish;
        }
    }
}
