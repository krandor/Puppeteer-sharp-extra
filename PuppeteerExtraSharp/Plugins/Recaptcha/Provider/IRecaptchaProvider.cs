namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider;

using System.Threading.Tasks;

public interface IRecaptchaProvider
{
    public Task<string> GetSolution(string key, string pageUrl, string proxyStr = null);
}
