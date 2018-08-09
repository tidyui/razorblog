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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using RazorBlog.Models;

namespace RazorBlog.Services
{
    public class Blog : IBlog
    {
        protected readonly IApi _api;
        protected readonly ISettings _settings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public Blog(IApi api, ISettings settings)
        {
            _api = api;
            _settings = settings;
        }

        /// <summary>
        /// Gets the current api.
        /// </summary>
        public IApi Api => _api;

        /// <summary>
        /// Gets the current blog settings.
        /// </summary>
        public ISettings Settings => _settings;

        /// <summary>
        /// Gets if the service contains archive data.
        /// </summary>
        public bool HasArchive => Archive != null;

        /// <summary>
        /// Gets if the service contains comment data.
        /// </summary>
        public bool HasComments => Comments != null;

        /// <summary>
        /// Gets if the service contains post data.
        /// </summary>
        public bool HasPost => Post != null;

        /// <summary>
        /// Gets/sets the optional archive data.
        /// </summary>
        public PostList Archive { get; set; }

        /// <summary>
        /// Gets/sets the optional comment data.
        /// </summary>
        public CommentList Comments { get; set; }

        /// <summary>
        /// Gets/sets the optional post data.
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        /// Gets the gravatar URL for the given email.
        /// </summary>
        /// <param name="email">The email</param>
        /// <param name="size">The requested image size</param>
        /// <returns>The gravatar url</returns>
        public string GetGravatar(string email, int size = 60)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(email.Trim().ToLower());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return $"https://www.gravatar.com/avatar/{sb.ToString().ToLower()}?s={size}&d=blank";
            }
        }
    }
}