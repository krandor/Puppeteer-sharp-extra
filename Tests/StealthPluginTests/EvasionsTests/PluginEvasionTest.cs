namespace Extra.Tests.StealthPluginTests.EvasionsTests;

using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

public class PluginEvasionTest: BrowserDefault
{
    [Fact]
    public async Task ShouldNotHaveModifications()
    {
        var stealthPlugin = new PluginEvasion();
        var page = await this.LaunchAndGetPage(stealthPlugin);

        await page.GoToAsync("https://google.com");
        
      
        var fingerPrint = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal(3, fingerPrint["plugins"].Count());
        Assert.Equal(4, fingerPrint["mimeTypes"].Count());
    }
}
