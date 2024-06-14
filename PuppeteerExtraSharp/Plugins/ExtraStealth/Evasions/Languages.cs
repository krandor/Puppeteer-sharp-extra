namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

public class Languages : PuppeteerExtraPlugin
{
    public StealthLanguagesOptions Options { get; }

    public Languages(StealthLanguagesOptions options = null) : base("stealth-language")
    {
        this.Options = options ?? new StealthLanguagesOptions("en-US", "en");
    }

    public override Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("Language.js");
        return Utils.EvaluateOnNewPage(page,script, this.Options.Languages);
    }
}

public class StealthLanguagesOptions : IPuppeteerExtraPluginOptions
{
    public object[] Languages { get; }

    public StealthLanguagesOptions(params string[] languages)
    {
        this.Languages = languages.Cast<object>().ToArray();
    }
}
