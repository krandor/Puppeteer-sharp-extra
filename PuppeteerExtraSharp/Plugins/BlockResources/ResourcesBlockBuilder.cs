namespace PuppeteerExtraSharp.Plugins.BlockResources;

using PuppeteerSharp;

public class ResourcesBlockBuilder
{
    private BlockRule Rule { get; set; } = new BlockRule();

    public ResourcesBlockBuilder BlockedResources(params ResourceType[] resources)
    {
        foreach (var resourceType in resources)
        {
            this.Rule.ResourceType.Add(resourceType);
        }

        return this;
    }

    public ResourcesBlockBuilder OnlyForPage(IPage page)
    {
        this.Rule.IPage = page;
        return this;
    }

    public ResourcesBlockBuilder ForUrl(string pattern)
    {
        this.Rule.SitePattern = pattern;
        return this;
    }

    internal BlockRule Build()
    {
        return this.Rule;
    }
}
