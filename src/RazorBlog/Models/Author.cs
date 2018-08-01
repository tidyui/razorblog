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
    public sealed class Author
    {
        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the author email.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Email { get; set; }
    }
}