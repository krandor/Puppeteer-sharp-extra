namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha;

using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha.Models;
using PuppeteerExtraSharp.Plugins.Recaptcha.RestClient;
using RestSharp;

internal class TwoCaptchaApi
{
    private readonly Plugins.Recaptcha.RestClient.RestClient _client = new Plugins.Recaptcha.RestClient.RestClient("https://rucaptcha.com");
    private readonly ProviderOptions _options;
    private readonly string _userKey;

    public TwoCaptchaApi(string userKey, ProviderOptions options)
    {
        this._userKey = userKey;
        this._options = options;
    }

    public async Task<TwoCaptchaResponse> CreateTaskAsync(string key, string pageUrl)
    {
        var result = await this._client.PostWithQueryAsync<TwoCaptchaResponse>("in.php", new Dictionary<string, string>()
        {
            ["key"] = this._userKey,
            ["googlekey"] = key,
            ["pageurl"] = pageUrl,
            ["json"] = "1",
            ["method"] = "userrecaptcha"
        });

        return result;
    }

    public async Task<RestResponse<TwoCaptchaResponse>> GetSolution(string id)
    {
        var request = new RestRequest("res.php") { Method = Method.Post };

        request.AddQueryParameter("id", id);
        request.AddQueryParameter("key", this._userKey);
        request.AddQueryParameter("action", "get");
        request.AddQueryParameter("json", "1");

        var result = await this._client.CreatePollingBuilder<TwoCaptchaResponse>(request).TriesLimit(this._options.PendingCount).ActivatePollingAsync(
            response => response.Data.request == "CAPCHA_NOT_READY" ? PollingAction.ContinuePolling : PollingAction.Break);

        return result;
    }
}