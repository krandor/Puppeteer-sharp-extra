namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha;

using System.Net.Http;
using System.Threading.Tasks;

public class AntiCaptcha : IRecaptchaProvider
{
    private readonly ProviderOptions _options;
    private readonly AntiCaptchaApi _api;
    public AntiCaptcha(string userKey, ProviderOptions options = null)
    {
        this._options = options ?? ProviderOptions.CreateDefaultOptions();
        this._api = new AntiCaptchaApi(userKey, this._options);
    }
    public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = null)
    {
        var task = await this._api.CreateTaskAsync(pageUrl, key);
        await System.Threading.Tasks.Task.Delay(this._options.StartTimeoutSeconds * 1000);
        var result = await this._api.PendingForResult(task.taskId);

        if (result.status != "ready" || result.solution is null || result.errorId != 0)
        {
            throw new HttpRequestException($"AntiCaptcha request ends with error - {result.errorId}");
        }

        return result.solution.gRecaptchaResponse;
    }
}
