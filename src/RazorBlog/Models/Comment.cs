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
using System.ComponentModel.DataAnnotations;

namespace RazorBlog.Models
{
    public sealed class Comment
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the post id.
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets/sets the author email.
        /// </summary>
        [Required, EmailAddress]
        [StringLength(128)]
        public string AuthorEmail { get; set; }

        /// <summary>
        /// Gets/sets the comment body.
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// Gets/sets if the comment is approved or not.
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>
        [Required]
        public DateTime Published { get; set; }

        /// <summary>
        /// Gets/sets the post.
        /// </summary>
        public Post Post { get; set; }
    }
}