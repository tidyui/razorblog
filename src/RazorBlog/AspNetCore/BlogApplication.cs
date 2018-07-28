/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/razorblog
 * 
 */

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using RazorBlog.Services;

namespace RazorBlog.AspNetCore
{
    public class BlogApplication
    {
        private int _fileCache = 0;
        private bool _withDependecies = false;

        /// <summary>
        /// Gets the application builder.
        /// </summary>
        public readonly IApplicationBuilder Builder;

        /// <summary>
        /// Gets the blog service.
        /// </summary>
        public readonly IBlogService Blog;

        /// <summary>
        /// Default internal constructor.
        /// </summary>
        /// <param name="builder">The current application builder</param>
        /// <param name="blog">The blog service</param>
        internal BlogApplication(IApplicationBuilder builder, IBlogService blog)
        {
            Builder = builder;
            Blog = blog;
        }

        /// <summary>
        /// Adds all framework dependencies needed for RazorBlog.
        /// </summary>
        /// <returns>The blog application</returns>
        public BlogApplication WithDependencies()
        {
            _withDependecies = true;

            return this;
        }

        /// <summary>
        /// Adds MaxAge caching to static files.
        /// </summary>
        /// <param name="days">The days the client can cache files</param>
        /// <returns>The blog application</returns>
        public BlogApplication WithFileCache(int days)
        {
            _fileCache = 60 * 60 * days;

            return this;
        }

        /// <summary>
        /// Starts the Razor Blog application.
        /// </summary>
        public void Run()
        {
            Builder.UseMiddleware<BlogMiddleware>();
            Builder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Pages", "Themes", Blog.Settings.Theme, "Assets")),
                RequestPath = "/Assets",
                OnPrepareResponse = ctx =>
                {
                    if (_fileCache > 0)
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            "public,max-age=" + _fileCache;
                    }
                }
            });

            if (_withDependecies)
            {
                Builder.UseMvc();
            }
        }
    }
}