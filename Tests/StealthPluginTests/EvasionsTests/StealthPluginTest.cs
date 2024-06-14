namespace Extra.Tests.StealthPluginTests.EvasionsTests;

using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using Xunit;

public class StealthPluginTest: BrowserDefault
{
    [Fact]
    public async Task Test()
    {
        var browser = await this.LaunchWithPluginAsync(new StealthPlugin());
        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://bot.sannysoft.com");
        await page.ScreenshotAsync("Stealth.png", new ScreenshotOptions()
        {
            FullPage = true,
            Type = ScreenshotType.Png,
        });
    }
}
