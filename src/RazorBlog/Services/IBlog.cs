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
    public interface IBlog
    {   
        /// <summary>
        /// Gets the current api.
        /// </summary>
        IApi Api { get; }

        /// <summary>
        /// Gets the current blog settings.
        /// </summary>
        ISettings Settings { get; }

        /// <summary>
        /// Gets if the service contains archive data.
        /// </summary>
        bool HasArchive { get; }

        /// <summary>
        /// Gets if the service contains comment data.
        /// </summary>
        bool HasComments { get; }

        /// <summary>
        /// Gets if the service contains post data.
        /// </summary>
        bool HasPost { get; }

        /// <summary>
        /// Gets/sets the optional archive data.
        /// </summary>
        PostList Archive { get; set; }

        /// <summary>
        /// Gets/sets the optional comment data.
        /// </summary>
        CommentList Comments { get; set; }

        /// <summary>
        /// Gets/sets the optional post data.
        /// </summary>
        Post Post { get; set; }

        /// <summary>
        /// Gets the gravatar URL for the given email.
        /// </summary>
        /// <param name="email">The email</param>
        /// <param name="size">The requested image size</param>
        /// <returns>The gravatar url</returns>
        string GetGravatar(string email, int size = 60);   
    }
}