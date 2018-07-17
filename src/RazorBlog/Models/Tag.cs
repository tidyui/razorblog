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

namespace RazorBlog.Models
{
    public sealed class Tag
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the id of the post this tag
        /// belongs to.
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Casts a string to a tag.
        /// </summary>
        /// <param name="title">The tag title</param>
        public static implicit operator Tag(string title)
        {
            return new Tag { Title = title };
        }
    }
}