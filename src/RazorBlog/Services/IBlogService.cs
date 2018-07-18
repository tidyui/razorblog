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
using RazorBlog.Models;

namespace RazorBlog.Services
{
    public interface IBlogService
    {   
        /// <summary>
        /// Gets the current blog settings.
        /// </summary>
        BlogSettings Settings { get; }

        /// <summary>
        /// Gets if the service contains archive data.
        /// </summary>
        bool HasArchive { get; }

        /// <summary>
        /// Gets if the service contains post data.
        /// </summary>
        bool HasPost { get; }

        /// <summary>
        /// Gets/sets the optional archive data.
        /// </summary>
        PostArchive Archive { get; set; }

        /// <summary>
        /// Gets/sets the optional post data.
        /// </summary>
        Post Post { get; set; }

        /// <summary>
        /// Gets the post with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post</returns>
        Task<Post> GetPostById(Guid id);

        /// <summary>
        /// Gets the post with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post</returns>
        Task<Post> GetPostBySlug(string slug);

        /// <summary>
        /// Gets the post archive.
        /// </summary>
        /// <param name="page">The current page of the archive</param>
        /// <param name="category">The optional category slug</param>
        /// <param name="tag">The optional tag slug</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive</returns>
        Task<Post[]> GetArchive(int page = 0, string category = null, string tag = null, int? year = null, int? month = null);

        /// <summary>
        /// Saves the given post.
        /// </summary>
        /// <param name="model">The post to save</param>
        /// <returns>The id of the post</returns>
        Task<Guid> SavePost(Post model);

        /// <summary>
        /// Generates a new slug from the given string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The slug</returns>
        string GenerateSlug(string str);
    }
}