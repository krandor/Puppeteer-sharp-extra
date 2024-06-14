namespace PuppeteerExtraSharp.Plugins.AnonymizeUa;

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp;

public class AnonymizeUaPlugin: PuppeteerExtraPlugin
{
    public AnonymizeUaPlugin(): base("anonymize-ua")
    {
    }

    private Func<string, string> _customAction;
    public void CustomizeUa(Func<string, string> uaAction)
    {
        this._customAction = uaAction;
    }

    public override async Task OnPageCreated(IPage page)
    {
        var ua = await page.Browser.GetUserAgentAsync();
        ua = ua.Replace("HeadlessChrome", "Chrome");

        var regex = new Regex(@"/\(([^)]+)\)/");
        ua = regex.Replace(ua, "(Windows NT 10.0; Win64; x64)");

        if (this._customAction != null)
        {
            ua = this._customAction(ua);
        }

        await page.SetUserAgentAsync(ua);
    }
}
