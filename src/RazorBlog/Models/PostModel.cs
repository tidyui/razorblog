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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBlog.Services;

namespace RazorBlog.Models
{
    /// <summary>
    /// Page model for a single post.
    /// </summary>
    public class PostModel : PageModel
    {
        /// <summary>
        /// The current blog service.
        /// </summary>
        protected readonly IBlogService _blog;

        /// <summary>
        /// Gets/sets the newly created comment.
        /// </summary>
        [BindProperty]
        public Models.Comment Comment { get; set; }        

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="blog">The blog service</param>
        public PostModel(IBlogService blog)
        {
            _blog = blog;
        }

        /// <summary>
        /// Creates a new comment for the current post.
        /// </summary>
        /// <returns>The updated page</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _blog.SaveComment(Comment);

            return LocalRedirect($"~{_blog.Settings.BlogPrefix}{_blog.Post.Slug}");
        }        
    }
}