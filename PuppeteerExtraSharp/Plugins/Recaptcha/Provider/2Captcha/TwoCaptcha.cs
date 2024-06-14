namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha;

using System.Net.Http;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha.Models;

public class TwoCaptcha : IRecaptchaProvider
{
    private readonly ProviderOptions _options;
    private readonly TwoCaptchaApi _api;

    public TwoCaptcha(string key, ProviderOptions options = null)
    {
        this._options = options ?? ProviderOptions.CreateDefaultOptions();
        this._api = new TwoCaptchaApi(key, this._options);
    }

    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = null)
    {
        var task = await this._api.CreateTaskAsync(key, pageUrl);

        this.ThrowErrorIfBadStatus(task);
        
        await Task.Delay(this._options.StartTimeoutSeconds * 1000);

        var result = await this._api.GetSolution(task.request);

        this.ThrowErrorIfBadStatus(result.Data);

        return result.Data.request;
    }

    private void ThrowErrorIfBadStatus(TwoCaptchaResponse response)
    {
        if (response.status != 1 || string.IsNullOrEmpty(response.request))
        {
            throw new HttpRequestException($"Two captcha request ends with error [{response.status}] {response.request}");
        }
    }
}
