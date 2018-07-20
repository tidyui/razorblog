/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/razorblog
 * 
 */

namespace RazorBlog.Models
{
    public sealed class CommentList
    {
        /// <summary>
        /// Gets/sets the available comments.
        /// </summary>
        public Comment[] Items { get; set; }

        /// <summary>
        /// Gets/sets the current page.
        /// </summary>
        public int Page { get; set; }
    }
}