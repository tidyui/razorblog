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
using System.Collections.Generic;

namespace RazorBlog.Models
{
    public sealed class Post
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the optional category id.
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the meta keywords.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the meta description.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>
        public DateTime? Published { get; set; }

        /// <summary>
        /// Gets/sets the last modified date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the optional category.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        /// <value></value>
        public IList<Tag> Tags { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Post()
        {
            Tags = new List<Tag>();
        }
    }
}