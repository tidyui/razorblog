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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorBlog.Services;

namespace RazorBlog.Http
{
    public class BlogApplicationService
    {
        /// <summary>
        /// Gets the application builder.
        /// </summary>
        public readonly IServiceCollection Services;

        /// <summary>
        /// Default internal constructor.
        /// </summary>
        /// <param name="builder">The current application builder</param>
        /// <param name="blog">The blog service</param>
        internal BlogApplicationService(IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            Services = services;

            Services.AddDbContext<Db>(options);
            Services.AddScoped<IBlogService, BlogService>();
        }

        /// <summary>
        /// Adds all framework dependencies needed for RazorBlog.
        /// </summary>
        /// <returns>The blog application</returns>
        public BlogApplicationService WithDependencies()
        {
            Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            return this;
        }
    }
}