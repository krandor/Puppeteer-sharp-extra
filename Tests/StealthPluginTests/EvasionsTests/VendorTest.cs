namespace Extra.Tests.StealthPluginTests.EvasionsTests;

using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

public class VendorTest: BrowserDefault
{
    [Fact]
    public async Task ShouldWork()
    {
        var plugin = new Vendor();
        var page = await this.LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var vendor = await page.EvaluateExpressionAsync<string>("navigator.vendor");
        Assert.Equal("Google Inc.", vendor);
    }
}
