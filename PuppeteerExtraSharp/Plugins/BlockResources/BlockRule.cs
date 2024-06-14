namespace PuppeteerExtraSharp.Plugins.BlockResources;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using PuppeteerSharp;

public class BlockRule
{
    public string SitePattern;
    public IPage IPage;
    public HashSet<ResourceType> ResourceType = new HashSet<ResourceType>();

    internal BlockRule()
    {

    }

    public bool IsRequestBlocked(IPage fromPage, IRequest request)
    {
        if (!this.IsResourcesBlocked(request.ResourceType))
        {
            return false;
        }

        return this.IsSiteBlocked(request.Url) || this.IsPageBlocked(fromPage);
    }


    public bool IsPageBlocked(IPage page)
    {
        return this.IPage != null && page.Equals(this.IPage);
    }

    public bool IsSiteBlocked(string siteUrl)
    {
        return !string.IsNullOrWhiteSpace(this.SitePattern) && Regex.IsMatch(siteUrl, this.SitePattern);
    }

    public bool IsResourcesBlocked(ResourceType resource)
    {
        return this.ResourceType.Contains(resource);
    }
}
