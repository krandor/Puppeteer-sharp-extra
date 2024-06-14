﻿namespace Extra.Tests.StealthPluginTests.EvasionsTests;

using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using Xunit;

public class PluginTest : BrowserDefault
{
    [Fact]
    public async Task HasMimeTypes()
    {
        var plugin = new PluginEvasion();
        var page = await this.LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var finger = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal(3, finger["plugins"].Count());
        Assert.Equal(4, finger["mimeTypes"].Count());
    }
}
