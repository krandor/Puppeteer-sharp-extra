namespace Extra.Tests;

using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public abstract class BrowserDefault : IDisposable
{
    private readonly List<IBrowser> _launchedBrowsers = new List<IBrowser>();

    protected BrowserDefault()
    {
    }

    public void Dispose()
    {
        foreach (var launchedBrowser in this._launchedBrowsers)
        {
            launchedBrowser.CloseAsync().Wait();
        }
    }

    protected LaunchOptions CreateDefaultOptions(string browserPath)
    {
        return new LaunchOptions()
        {
            Headless = false, // Constants.Headless,
            ExecutablePath = browserPath,
        };
    }

    protected async Task<IPage> LaunchAndGetPage(PuppeteerExtraPlugin plugin = null)
    {
        IBrowser browser = null;
        if (plugin != null)
        {
            browser = await this.LaunchWithPluginAsync(plugin);
        }
        else
        {
            browser = await this.LaunchAsync();
        }

        var page = (await browser.PagesAsync())[0];

        return page;
    }

    protected async Task<IBrowser> LaunchAsync(LaunchOptions options = null)
    {
        options ??= this.CreateDefaultOptions(await this.DownloadChromeIfNotExists());

        var browser = await Puppeteer.LaunchAsync(options);
        this._launchedBrowsers.Add(browser);
        return browser;
    }

    protected async Task<IBrowser> LaunchWithPluginAsync(PuppeteerExtraPlugin plugin, LaunchOptions options = null)
    {
        var extra = new PuppeteerExtra().Use(plugin);
        options ??= this.CreateDefaultOptions(await this.DownloadChromeIfNotExists());

        var browser = await extra.LaunchAsync(options);
        this._launchedBrowsers.Add(browser);
        return browser;
    }

    private async Task<string> DownloadChromeIfNotExists()
    {
        await new BrowserFetcher(new BrowserFetcherOptions()).DownloadAsync(BrowserTag.Latest);

        var supportedBrowser = SupportedBrowser.Chrome;

        using var browserFetcher = new BrowserFetcher(supportedBrowser);
        var installedBrowsers = browserFetcher.GetInstalledBrowsers();
        InstalledBrowser selectedBrowser;
        if (!installedBrowsers.Any())
        {
            selectedBrowser = await browserFetcher.DownloadAsync(BrowserTag.Latest);
        }
        else
        {
            selectedBrowser = installedBrowsers.FirstOrDefault();
        }

        return selectedBrowser.GetExecutablePath();
    }
}