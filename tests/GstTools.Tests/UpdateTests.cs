using GstTools;

namespace GstTools.Tests;

public sealed class UpdateTests
{
    private static readonly Executable Local = new("GstLosGenerator-v0.3.2-win-x64.exe", new Version(0, 3, 2));

    private static Release Published(string tag) => new(tag, ToolApps.VersionFromTag(tag)!, "https://github.com/x/y/releases/tag/" + tag);

    [Fact]
    public void ReadRelease_GitHubResponse_ReturnsRelease()
    {
        var release = ToolApps.ReadRelease("""{"tag_name":"v0.3.3","html_url":"https://github.com/nsuarez75/GST-LoS-Generator/releases/tag/v0.3.3","name":"v0.3.3"}""");

        Assert.Equal(new Release("v0.3.3", new Version(0, 3, 3), "https://github.com/nsuarez75/GST-LoS-Generator/releases/tag/v0.3.3"), release);
    }

    [Theory]
    [InlineData("""{"message":"Not Found"}""")]
    [InlineData("""{"tag_name":"latest","html_url":"https://github.com"}""")]
    [InlineData("no es json")]
    public void ReadRelease_UnexpectedResponse_ReturnsNull(string json) => Assert.Null(ToolApps.ReadRelease(json));

    [Theory]
    [InlineData("v0.3.3", true)]
    [InlineData("v0.10.0", true)]
    [InlineData("v0.3.2", false)]
    [InlineData("v0.3.1", false)]
    public void HasNewer_PublishedVersion_TrueOnlyIfNewer(string tag, bool newer) =>
        Assert.Equal(newer, ToolApps.HasNewer(Local, Published(tag)));

    [Fact]
    public void HasNewer_MissingVersionOrRelease_ReturnsFalse()
    {
        Assert.False(ToolApps.HasNewer(Local with { Version = null }, Published("v9.0.0")));
        Assert.False(ToolApps.HasNewer(null, Published("v9.0.0")));
        Assert.False(ToolApps.HasNewer(Local, null));
    }

    [Fact]
    public void LatestReleaseApi_Repository_UsesGitHubApi() =>
        Assert.All(ToolApps.All, p => Assert.StartsWith("https://api.github.com/repos/nsuarez75/", p.LatestReleaseApi));
}
