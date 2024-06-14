namespace PuppeteerExtraSharp.Plugins.Recaptcha;

using System.Threading.Tasks;
using System.Web;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

public class Recaptcha
{
    private readonly IRecaptchaProvider _provider;
    private readonly CaptchaOptions _options;

    public Recaptcha(IRecaptchaProvider provider, CaptchaOptions options)
    {
        this._provider = provider;
        this._options = options;
    }

    public async Task<RecaptchaResult> Solve(IPage page)
    {
        try
        {
            var key = await this.GetKeyAsync(page);
            var solution = await this.GetSolutionAsync(key, page.Url);
            await this.WriteToInput(page, solution);

            return new RecaptchaResult()
            {
                IsSuccess = true
            };
        }
        catch (CaptchaException ex)
        {
            return new RecaptchaResult()
            {
                Exception = ex,
                IsSuccess = false
            };
        }

    }

    public async Task<string> GetKeyAsync(IPage page)
    {
        var element =
            await page.QuerySelectorAsync("iframe[src^='https://www.google.com/recaptcha/api2/anchor'][name^=\"a-\"]");

        if (element == null)
        {
            throw new CaptchaException(page.Url, "Recaptcha key not found!");
        }

        var src = await element.GetPropertyAsync("src");

        if (src == null)
        {
            throw new CaptchaException(page.Url, "Recaptcha key not found!");
        }

        var key = HttpUtility.ParseQueryString(src.ToString()).Get("k");
        return key;
    }

    public async Task<string> GetSolutionAsync(string key, string urlPage)
    {
        return await this._provider.GetSolution(key, urlPage);
    }

    public async Task WriteToInput(IPage page, string value)
    {
        await page.EvaluateFunctionAsync(
              $"() => {{document.getElementById('g-recaptcha-response').innerHTML='{value}'}}");


        var script = ResourcesReader.ReadFile(this.GetType().Namespace + ".Scripts.EnterRecaptchaCallBackScript.js");

        try
        {
            await page.EvaluateFunctionAsync($@"(value) => {{{script}}}", value);
        }
        catch
        {
            // ignored
        }
    }
}
