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
using Microsoft.AspNetCore.Html;
using Markdig;

namespace RazorBlog.Models
{
    public sealed class MarkdownString
    {
        /// <summary>
        /// Gets/sets the markdown value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets the markdown converted to HTML.
        /// </summary>
        /// <returns>The HTML value</returns>
        public HtmlString ToHtml()
        {
            return new HtmlString(Markdown.ToHtml(Value));
        }

        /// <summary>
        /// Casts a string to markdown.
        /// </summary>
        /// <param name="str">The string</param>
        public static implicit operator MarkdownString(string str)
        {
            return new MarkdownString { Value = str };
        }

        /// <summary>
        /// Casts markdown to a string.
        /// </summary>
        /// <param name="md">The markdown value</param>
        public static implicit operator String(MarkdownString md)
        {
            return md.Value;
        }
    }
}