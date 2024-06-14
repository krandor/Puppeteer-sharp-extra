namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

public class ContentWindow : PuppeteerExtraPlugin
{
    public ContentWindow() : base("Iframe.ContentWindow") { }

    public override List<PluginRequirements> Requirements { get; set; } = new()
    {
        PluginRequirements.RunLast
    };

    public override Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("ContentWindow.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}