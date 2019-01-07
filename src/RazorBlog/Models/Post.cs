/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/razorblog
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Markdig;

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
        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        [StringLength(128)]
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the meta keywords.
        /// </summary>
        [StringLength(128)]
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the meta description.
        /// </summary>
        [StringLength(255)]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the excerpt.
        /// </summary>
        public MarkdownString Excerpt { get; set; }

        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        public MarkdownString Body { get; set; }

        /// <summary>
        /// Gets the number of available comments.
        /// </summary>
        public int CommentCount { get; internal set; }

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>
        public DateTime? Published { get; set; }

        /// <summary>
        /// Gets/sets the last modified date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the author information.
        /// </summary>
        public Author Author { get; set; }

        /// <summary>
        /// Gets/sets the optional category.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets/sets the available comments.
        /// </summary>
        public IList<Comment> Comments { get; set; }

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
            Author = new Author();
            Comments = new List<Comment>();
            Tags = new List<Tag>();
        }
    }
}