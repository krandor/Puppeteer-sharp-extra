namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

using System.Threading.Tasks;
using PuppeteerSharp;

public class ChromeSci: PuppeteerExtraPlugin
{
    public ChromeSci(): base("stealth_sci") { }

    public override Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("SCI.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}
