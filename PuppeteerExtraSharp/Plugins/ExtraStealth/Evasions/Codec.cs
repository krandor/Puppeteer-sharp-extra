namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

using System.Threading.Tasks;
using PuppeteerSharp;

public class Codec : PuppeteerExtraPlugin
{
    public Codec() : base("stealth-codec") { }

    public override Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("Codec.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}
