/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/razorblog
 * 
 */

namespace RazorBlog.Services
{
    public interface ISettings
    {
        /// <summary>
        /// Gets/sets the main blog title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets/sets the main blog description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets/sets the URL prefix for the entire blog.
        /// </summary>
        string BlogPrefix { get; set; }

        /// <summary>
        /// Gets/sets the URL slug for the blog archive.
        /// </summary>
        string ArchiveSlug { get; set; }

        /// <summary>
        /// Gets/sets the page size for the post listing.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Gets/sets the currently active theme.
        /// </summary>
        string Theme { get; set; }
    }
}