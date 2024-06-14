namespace PuppeteerExtraSharp.Plugins.Recaptcha;

using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider;
using PuppeteerSharp;

public class RecaptchaPlugin : PuppeteerExtraPlugin
{
    private readonly Recaptcha _recaptcha;

    public RecaptchaPlugin(IRecaptchaProvider provider, CaptchaOptions opt = null) : base("recaptcha")
    {
        this._recaptcha = new Recaptcha(provider, opt ?? new CaptchaOptions());
    }

    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page)
    {
        return await this._recaptcha.Solve(page);
    }

    public override async Task OnPageCreated(IPage page)
    {
        await page.SetBypassCSPAsync(true);
    }
}
