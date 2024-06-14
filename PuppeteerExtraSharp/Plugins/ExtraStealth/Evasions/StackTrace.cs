namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

using System.Threading.Tasks;
using PuppeteerSharp;

public class StackTrace : PuppeteerExtraPlugin
{
    public StackTrace() : base("stealth-stackTrace") { }

    public override async Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("Stacktrace.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}
