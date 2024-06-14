namespace PuppeteerExtraSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;

public class PuppeteerExtra
{
    private List<PuppeteerExtraPlugin> _plugins = new List<PuppeteerExtraPlugin>();

    public PuppeteerExtra Use(PuppeteerExtraPlugin plugin)
    {
        this._plugins.Add(plugin);
        this.ResolveDependencies(plugin);
        plugin.OnPluginRegistered();
        return this;
    }

    public async Task<IBrowser> LaunchAsync(LaunchOptions options)
    {
        this._plugins.ForEach(e => e.BeforeLaunch(options));
        var browser = await Puppeteer.LaunchAsync(options);
        this._plugins.ForEach(e => e.AfterLaunch(browser));
        await this.OnStart(new BrowserStartContext()
        {
            StartType = StartType.Launch,
            IsHeadless = options.Headless
        }, browser);
        return browser;
    }

    public async Task<IBrowser> ConnectAsync(ConnectOptions options)
    {
        this._plugins.ForEach(e => e.BeforeConnect(options));
        var browser = await Puppeteer.ConnectAsync(options);
        this._plugins.ForEach(e => e.AfterConnect(browser));
        await this.OnStart(new BrowserStartContext()
        {
            StartType = StartType.Connect
        }, browser);
        return browser;
    }

    public T GetPlugin<T>() where T : PuppeteerExtraPlugin
    {
        return (T)this._plugins.FirstOrDefault(e => e.GetType() == typeof(T));
    }

    private async Task OnStart(BrowserStartContext context, IBrowser browser)
    {
        this.OrderPlugins();
        this.CheckPluginRequirements(context);
        await this.Register(browser);
    }

    private void ResolveDependencies(PuppeteerExtraPlugin plugin)
    {
        var dependencies = plugin.GetDependencies()?.ToList();
        if (dependencies is null || !dependencies.Any())
        {
            return;
        }

        foreach (var puppeteerExtraPlugin in dependencies)
        {
            this.Use(puppeteerExtraPlugin);

            var plugDependencies = puppeteerExtraPlugin.GetDependencies()?.ToList();

            if (plugDependencies != null && plugDependencies.Any())
            {
                plugDependencies.ForEach(this.ResolveDependencies);
            }
        }
    }

    private void OrderPlugins()
    {
        this._plugins = this._plugins.OrderBy(e => e.Requirements?.Contains(PluginRequirements.RunLast)).ToList();
    }

    private void CheckPluginRequirements(BrowserStartContext context)
    {
        foreach (var puppeteerExtraPlugin in this._plugins)
        {
            if (puppeteerExtraPlugin.Requirements is null)
            {
                continue;
            }

            foreach (var requirement in puppeteerExtraPlugin.Requirements)
            {
                switch (context.StartType)
                {
                    case StartType.Launch when requirement == PluginRequirements.HeadFul && context.IsHeadless:
                        throw new NotSupportedException(
                            $"Plugin - {puppeteerExtraPlugin.Name} is not supported in headless mode");
                    case StartType.Connect when requirement == PluginRequirements.Launch:
                        throw new NotSupportedException(
                            $"Plugin - {puppeteerExtraPlugin.Name} doesn't support connect");
                }
            }
        }
    }

    private async Task Register(IBrowser browser)
    {
        var pages = await browser.PagesAsync();

        browser.TargetCreated += async (sender, args) =>
        {
            this._plugins.ForEach(e => e.OnTargetCreated(args.Target));
            if (args.Target.Type == TargetType.Page)
            {
                var page = await args.Target.PageAsync();
                this._plugins.ForEach(async e => await e.OnPageCreated(page));
            }
        };

        foreach (var puppeteerExtraPlugin in this._plugins)
        {
            browser.TargetChanged += (sender, args) => puppeteerExtraPlugin.OnTargetChanged(args.Target);
            browser.TargetDestroyed += (sender, args) => puppeteerExtraPlugin.OnTargetDestroyed(args.Target);
            browser.Disconnected += (sender, args) => puppeteerExtraPlugin.OnDisconnected();
            browser.Closed += (sender, args) => puppeteerExtraPlugin.OnClose();
            foreach (var page in pages)
            {
                await puppeteerExtraPlugin.OnPageCreated(page);
            }
        }
    }
}