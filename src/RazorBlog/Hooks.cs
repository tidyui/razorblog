/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/razorblog
 * 
 */

using System.Xml;

namespace RazorBlog
{
    /// <summary>
    /// The different hooks available.
    /// </summary>
    public static class Hooks
    {
        /// <summary>
        /// Delegates used by the hooks.
        /// </summary>
        public static class Delegates
        {
            public delegate void XmlGenerateDelegate(XmlWriter writer);
        }

        /// <summary>
        /// The hooks available for sitemap generation.
        /// </summary>
        public static class Sitemap
        {
            /// <summary>
            /// Called when the sitemap is loaded and generated.
            /// </summary>
            public static Delegates.XmlGenerateDelegate OnLoad;
        }
    }
}