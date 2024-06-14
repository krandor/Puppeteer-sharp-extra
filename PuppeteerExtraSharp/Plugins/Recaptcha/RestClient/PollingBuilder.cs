namespace PuppeteerExtraSharp.Plugins.Recaptcha.RestClient;

using System;
using System.Threading.Tasks;
using RestSharp;

public class PollingBuilder<T>
{
    private readonly RestSharp.RestClient _client;
    private readonly RestSharp.RestRequest _request;
    private int _timeout = 5;
    private int _limit = 5;
    public PollingBuilder(RestSharp.RestClient client, RestRequest request)
    {
        this._client = client;
        this._request = request;
    }

    public PollingBuilder<T> WithTimeoutSeconds(int timeout)
    {
        this._timeout = timeout;
        return this;
    }

    public PollingBuilder<T> TriesLimit(int limit)
    {
        this._limit = limit;
        return this;
    }

    public async Task<RestResponse<T>> ActivatePollingAsync(Func<RestResponse<T>, PollingAction> resultDelegate)
    {
        var response = await this._client.ExecuteAsync<T>(this._request);

        if (resultDelegate(response) == PollingAction.Break || this._limit <= 1)
        {
            return response;
        }

        await Task.Delay(this._timeout * 1000);
        this._limit -= 1;

        return await this.ActivatePollingAsync(resultDelegate);
    }
}

public enum PollingAction
{
    ContinuePolling,
    Break
}
