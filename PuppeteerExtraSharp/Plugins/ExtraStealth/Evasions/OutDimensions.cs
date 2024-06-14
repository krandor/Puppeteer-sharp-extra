namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

using System.Threading.Tasks;
using PuppeteerSharp;

public class OutDimensions : PuppeteerExtraPlugin
{
    public OutDimensions() : base("stealth-dimensions") { }

    public override async Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("Outdimensions.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}
