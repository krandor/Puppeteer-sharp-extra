namespace PuppeteerExtraSharp.Plugins.Recaptcha;

using System;

public class CaptchaException: Exception
{
    public CaptchaException(string pageUrl, string content)
    {
        this.PageUrl = pageUrl;
        this.Content = content;
    }

    public string PageUrl { get; set; }
    public string Content { get; set; }
}
