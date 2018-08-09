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
    public sealed class Setting
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Key { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Value { get; set; }
    }
}