namespace PuppeteerExtraSharp.Plugins.ExtraStealth;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;

public class StealthPlugin : PuppeteerExtraPlugin
{
    private readonly IPuppeteerExtraPluginOptions[] _options;
    private readonly List<PuppeteerExtraPlugin> _standardEvasions;

    public StealthPlugin(params IPuppeteerExtraPluginOptions[] options) : base("stealth")
    {
        this._options = options;
        this._standardEvasions = this.GetStandardEvasions();
    }

    private List<PuppeteerExtraPlugin> GetStandardEvasions()
    {
        return new List<PuppeteerExtraPlugin>()
    {
        new WebDriver(),
        // new ChromeApp(),
        new ChromeSci(),
        new ChromeRuntime(),
        new Codec(),
        new Languages(this.GetOptionByType<StealthLanguagesOptions>()),
        new OutDimensions(),
        new Permissions(),
        new UserAgent(),
        new Vendor(this.GetOptionByType<StealthVendorSettings>()),
        new WebGl(this.GetOptionByType<StealthWebGLOptions>()),
        new PluginEvasion(),
        new StackTrace(),
        new HardwareConcurrency(this.GetOptionByType<StealthHardwareConcurrencyOptions>()),
        new ContentWindow(),
        new SourceUrl()
    };
    }

    public override ICollection<PuppeteerExtraPlugin> GetDependencies() => this._standardEvasions;

    public override async Task OnPageCreated(IPage page)
    {
        var utilsScript = Utils.GetScript("Utils.js");
        await page.EvaluateExpressionOnNewDocumentAsync(utilsScript);
    }

    private T GetOptionByType<T>() where T : IPuppeteerExtraPluginOptions
    {
        return this._options.OfType<T>().FirstOrDefault();
    }

    public void RemoveEvasionByType<T>() where T : PuppeteerExtraPlugin
    {
        this._standardEvasions.RemoveAll(ev => ev is T);
    }
}