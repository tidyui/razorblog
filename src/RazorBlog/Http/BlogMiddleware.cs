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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RazorBlog.Models;
using RazorBlog.Services;

namespace RazorBlog.Http
{
    public class BlogMiddleware
    {
        protected readonly RequestDelegate _next;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public BlogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <param name="service">The blog service</param>
        public virtual async Task Invoke(HttpContext context, IBlogService service)
        {
            var url = context.Request.Path.HasValue ? 
                context.Request.Path.Value.ToLower() : "";

            //
            // Check if this request is for the blog startpage
            //
            if (string.IsNullOrEmpty(url) || url == service.Settings.BlogPrefix)
            {
                service.Archive = await service.GetArchive();
                context.Request.Path = new PathString($"/Themes/{service.Settings.Theme}/Pages/_Archive");
            }
            //
            // Check if this request is the for the blog archive
            //
            else if (url.StartsWith(service.Settings.ArchiveSlug))
            {
                var segments = url.Substring(1).Split('/');

                if (segments.Length > 1)
                {
                    int page = 1;
                    int? year = null;
                    int? month = null;
                    string category = null;
                    string tag = null;
                    bool foundCategory = false;
                    bool foundTag = false;
                    bool foundPage = false;

                    for (var n = 1; n < segments.Length; n++)
                    {
                        if (segments[n] == "category" && !foundPage)
                        {
                            foundCategory = true;
                            continue;
                        }

                        if (segments[n] == "tag" && !foundPage)
                        {
                            foundTag = true;
                            continue;
                        }

                        if (segments[n] == "page")
                        {
                            foundPage = true;
                            continue;
                        }

                        if (foundCategory)
                        {
                            category = segments[n];
                            foundCategory = false;
                        }

                        if (foundTag)
                        {
                            tag = segments[n];
                            foundTag = false;
                        }

                        if (foundPage)
                        {
                            try
                            {
                                page = Convert.ToInt32(segments[n]);
                            }
                            catch { }
                            break;
                        }

                        if (!year.HasValue)
                        {
                            try
                            {
                                year = Convert.ToInt32(segments[n]);

                                if (year.Value > DateTime.Now.Year)
                                    year = DateTime.Now.Year;
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                month = Math.Max(Math.Min(Convert.ToInt32(segments[n]), 12), 1);
                            }
                            catch { }
                        }
                    }
                    service.Archive = await service.GetArchive(page, category, tag, year, month);
                    context.Request.Path = new PathString($"/Themes/{service.Settings.Theme}/Pages/_Archive");
                }
            }
            //
            // Check if this request is for getting comments
            //
            else if (url.StartsWith("/comments/"))
            {
                var segments = url.Substring(1).Split('/');
                var postId = Guid.Empty;
                var page = 0;

                if (segments.Length > 1)
                {
                    postId = new Guid(segments[1]);

                    if (segments.Length > 2)
                        page = Convert.ToInt32(segments[2]);
                    service.Comments = new CommentList
                    {
                        Items = await service.GetComments(postId, page),
                        Page = page
                    };
                    context.Request.Path = new PathString($"/Themes/{service.Settings.Theme}/Pages/_Comments");                        
                }
            }
            //
            // Check if this is a request for a single post
            //
            else if (!url.StartsWith("/assets/"))
            {
                var slug = url.Replace(service.Settings.BlogPrefix, "");

                var post = await service.GetPost(slug);

                if (post != null)
                {
                    service.Post = post;
                    context.Request.Path = new PathString($"/Themes/{service.Settings.Theme}/Pages/_Post");
                }
            }
            await _next.Invoke(context);
        }
    }
}