namespace Extra.Tests.StealthPluginTests.EvasionsTests;

using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

public class PermissionsTest: BrowserDefault
{
    [Fact]
    public async Task ShouldBeDeniedInHttpSite()
    {
        var plugin = new Permissions();
        var page = await this.LaunchAndGetPage(plugin);
        await page.GoToAsync("http://info.cern.ch/");

        var finger = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal("denied", finger["permissions"]["state"]);
        Assert.Equal("denied", finger["permissions"]["permission"]);
    }
}
