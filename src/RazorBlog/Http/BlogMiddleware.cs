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

            if (!url.StartsWith("/assets/"))
            {
                if (string.IsNullOrEmpty(url) || url == service.Settings.BlogPrefix)
                {
                    service.Archive = new PostArchive
                    {
                        Posts = await service.GetArchive()
                    };
                    context.Request.Path = new PathString($"/Themes/{service.Settings.Theme}/Pages/_Archive");
                }
                else
                {
                    var slug = url.Replace(service.Settings.BlogPrefix, "");

                    var post = await service.GetPostBySlug(slug);

                    if (post != null)
                    {
                        service.Post = post;
                        context.Request.Path = new PathString($"/Themes/{service.Settings.Theme}/Pages/_Post");
                    }
                }
            }
            await _next.Invoke(context);
        }
    }
}