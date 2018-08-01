/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/razorblog
 * 
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;
using Microsoft.SyndicationFeed.Rss;
using RazorBlog.Models;
using RazorBlog.Services;

namespace RazorBlog.AspNetCore
{
    public class FeedMiddleware
    {
        protected readonly RequestDelegate _next;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public FeedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <param name="blog">The blog service</param>
        public virtual async Task Invoke(HttpContext context, IBlogService blog, Db db)
        {
            var url = context.Request.Path.HasValue ? 
                context.Request.Path.Value.ToLower() : "";

            if (url.StartsWith("/feed"))
            {
                using (var xml = XmlWriter.Create(context.Response.Body, new XmlWriterSettings { Async = true, Indent = true }))
                {
                    var host = context.Request.Scheme + "://" + context.Request.Host;
                    var latest = await db.Posts
                        .Where(p => p.Published <= DateTime.Now.ToUniversalTime())
                        .MaxAsync(p => p.Published);
                    var feed = await GetWriter(xml, blog, url, host, latest);

                    if (feed != null)
                    {
                        context.Response.ContentType = "application/xml";

                        var posts = db.Posts
                            .Include(p => p.Category)
                            .Include(p => p.Tags)
                            .Where(p => p.Published <= DateTime.Now.ToUniversalTime())
                            .OrderByDescending(p => p.LastModified).ThenBy(p => p.Published);

                        foreach (Models.Post post in posts)
                        {
                            var item = new AtomEntry
                            {
                                Title = post.Title,
                                Description = post.Body.ToHtml().Value,
                                Id = $"{host}/{post.Slug}",
                                Published = post.Published.Value,
                                LastUpdated = post.LastModified,
                                ContentType = "html",
                            };

                            // Add category
                            item.AddCategory(new SyndicationCategory(post.Category.Title));

                            // Add tags
                            foreach (var tag in post.Tags)
                            {
                                item.AddCategory(new SyndicationCategory(tag.Title));
                            }

                            //
                            // TODO: Post needs an author
                            //
                            item.AddContributor(new SyndicationPerson(post.Author.Name, post.Author.Email));
                            item.AddLink(new SyndicationLink(new Uri(item.Id)));

                            await feed.Write(item);                            
                        }
                    }
                }
            }
            await _next.Invoke(context);
        }

        /// <summary>
        /// Creates the feed writer and writes the initial headers.
        /// </summary>
        /// <param name="writer">The current xml writer</param>
        /// <param name="blog">The blog service</param>
        /// <param name="url">The currently requested url</param>
        /// <param name="host">The host name</param>
        /// <returns>The feed writer</returns>
        private async Task<ISyndicationFeedWriter> GetWriter(XmlWriter writer, IBlogService blog, string url, string host, DateTime? latest)
        {
            var segments = url.Substring(1).Split('/');

            if (latest.HasValue && segments.Length == 2)
            {
                if (segments[1] == "rss")
                {
                    var rss = new RssFeedWriter(writer);

                    // Write feed headers
                    await rss.WriteTitle(blog.Settings.Title);
                    await rss.WriteDescription(blog.Settings.Description);
                    await rss.WriteGenerator("RazorBlog");
                    await rss.WriteValue("link", host);

                    return rss;
                }
                else if (segments[1] == "atom")
                {
                    var atom = new AtomFeedWriter(writer);

                    // Write feed headers
                    await atom.WriteTitle(blog.Settings.Title);
                    await atom.WriteId(host);
                    await atom.WriteSubtitle(blog.Settings.Description);
                    await atom.WriteGenerator("RazorBlog", "https://github.com/tidyui/razorblog", "0.1");
                    await atom.WriteValue("updated", latest.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            
                    return atom;
                }
            }
            return null;
        }
    }
}