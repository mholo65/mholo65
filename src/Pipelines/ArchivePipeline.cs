﻿using System.Linq;
using site.Extensions;
using Statiq.Common;
using Statiq.Core;
using Statiq.Handlebars;

namespace site.Pipelines
{
    public class ArchivePipeline : AppliedLayoutPipeline
    {
        public ArchivePipeline()
        {
            Dependencies.Add(nameof(BlogPostPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("_archive.hbs")
            };

            ProcessModules = new ModuleList
            {
                new SetDestination(Config.FromValue(new NormalizedPath("./posts/index.html"))),
                new RenderHandlebars()
                    .WithModel(Config.FromContext(context => new
                    {
                        groups = context.Outputs.FromPipeline(nameof(BlogPostPipeline))
                            .GroupBy(x => x.GetDateTime("Published").Year)
                            .OrderByDescending(x => x.Key)
                            .Select(group => new
                            {
                                key = group.Key,
                                posts = group
                                    .OrderByDescending(x => x.GetDateTime("Published"))
                                    .Select(x => x.AsPost(context)),
                            })
                    }))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}