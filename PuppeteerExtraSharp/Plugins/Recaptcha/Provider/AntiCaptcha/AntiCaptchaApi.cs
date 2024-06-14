namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha;

using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.Models;
using PuppeteerExtraSharp.Plugins.Recaptcha.RestClient;
using RestSharp;

public class AntiCaptchaApi
{
    private readonly Plugins.Recaptcha.RestClient.RestClient _client = new Plugins.Recaptcha.RestClient.RestClient("http://api.anti-captcha.com");
    private readonly ProviderOptions _options;
    private readonly string _userKey;

    public AntiCaptchaApi(string userKey, ProviderOptions options)
    {
        this._userKey = userKey;
        this._options = options;
    }

    public Task<AntiCaptchaTaskResult> CreateTaskAsync(string pageUrl, string key, CancellationToken token = default)
    {
        var content = new AntiCaptchaRequest()
        {
            clientKey = this._userKey,
            task = new AntiCaptchaTask()
            {
                type = "NoCaptchaTaskProxyless",
                websiteURL = pageUrl,
                websiteKey = key
            }
        };

        var result = this._client.PostWithJsonAsync<AntiCaptchaTaskResult>("createTask", content, token);
        return result;
    }

    public async Task<TaskResultModel> PendingForResult(int taskId, CancellationToken token = default)
    {
        var content = new RequestForResultTask()
        {
            clientKey = this._userKey,
            taskId = taskId
        };

        var request = new RestRequest("getTaskResult");
        request.AddJsonBody(content);
        request.Method = Method.Post;

        var result = await this._client.CreatePollingBuilder<TaskResultModel>(request).TriesLimit(this._options.PendingCount)
            .WithTimeoutSeconds(5).ActivatePollingAsync(
                response =>
                {
                    if (response.Data.status == "ready" || response.Data.errorId != 0)
                    {
                        return PollingAction.Break;
                    }

                    return PollingAction.ContinuePolling;
                });
        return result.Data;
    }
}