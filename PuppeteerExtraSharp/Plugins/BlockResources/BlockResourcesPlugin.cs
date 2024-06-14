﻿namespace PuppeteerExtraSharp.Plugins.BlockResources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

public class BlockResourcesPlugin : PuppeteerExtraPlugin
{
    public readonly List<BlockRule> BlockResources = new List<BlockRule>();

    public BlockResourcesPlugin(IEnumerable<ResourceType> blockResources = null): base("block-resources")
    {
        if (blockResources != null)
        {
            this.AddRule(builder => builder.BlockedResources(blockResources.ToArray()));
        }
    }

    public BlockRule AddRule(Action<ResourcesBlockBuilder> builderAction)
    {
        var builder = new ResourcesBlockBuilder();
        builderAction(builder);

        var rule = builder.Build();
        this.BlockResources.Add(builder.Build());

        return rule;
    }

    public BlockResourcesPlugin RemoveRule(BlockRule rule)
    {
        this.BlockResources.Remove(rule);
        return this;
    }


    public override async Task OnPageCreated(IPage page)
    {
        await page.SetRequestInterceptionAsync(true);
        page.Request += (sender, args) => this.OnPageRequest(page, args);

    }


    private async void OnPageRequest(IPage sender, RequestEventArgs e)
    {
        if (this.BlockResources.Any(rule => rule.IsRequestBlocked(sender, e.Request)))
        {
            await e.Request.AbortAsync();
            return;
        }

        await e.Request.ContinueAsync();
    }


    public override void BeforeLaunch(LaunchOptions options)
    {
        options.Args = options.Args.Append("--site-per-process").Append("--disable-features=IsolateOrigins").ToArray();
    }
}
