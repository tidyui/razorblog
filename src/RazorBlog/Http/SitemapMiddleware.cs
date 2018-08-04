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
using RazorBlog.Models;
using RazorBlog.Services;

namespace RazorBlog.Http
{
    public class SitemapMiddleware
    {
        protected readonly RequestDelegate _next;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public SitemapMiddleware(RequestDelegate next)
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

            if (url == "/sitemap.xml")
            {
                var host = context.Request.Scheme + "://" + context.Request.Host;

                using (var xml = XmlWriter.Create(context.Response.Body, new XmlWriterSettings { Indent = true }))
                {
                    xml.WriteStartDocument();
                    xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    var posts = db.Posts
                        .Where(p => p.Published <= DateTime.Now.ToUniversalTime())
                        .OrderByDescending(p => p.LastModified).ThenBy(p => p.Published);

                    foreach (Models.Post post in posts)
                    {
                        var lastMod = new[] { post.Published.Value, post.LastModified };

                        xml.WriteStartElement("url");
                        xml.WriteElementString("loc", $"{host}/{post.Slug}");
                        xml.WriteElementString("lastmod", lastMod.Max().ToString("yyyy-MM-ddThh:mmzzz"));
                        xml.WriteEndElement();
                    }

                    // Execute sitemap generation hooks
                    Hooks.Sitemap.OnLoad?.Invoke(xml);

                    xml.WriteEndElement();
                }
                context.Response.ContentType = "application/xml";
            }
            await _next.Invoke(context);
        }
    }
}