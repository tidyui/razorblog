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
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RazorBlog.Models;

namespace RazorBlog.Services
{
    public class Settings : ISettings
    {
        protected readonly Db _db;

        private static string _title;
        private static string _description;
        private static string _blogPrefix;
        private static string _archiveSlug;
        private static int? _pageSize;
        private static string _theme;

        /// <summary>
        /// Gets/sets the main blog title.
        /// </summary>
        public virtual string Title { 
            get
            {
                if (_title == null)
                {
                    _title = Get<string>("RazorBlog_Title", "RazorBlog");
                }
                return _title;
            }
            set
            {
                Set("RazorBlog_Title", value);
                _title = value;
            }
        } 

        /// <summary>
        /// Gets/sets the main blog description.
        /// </summary>
        public virtual string Description 
        {
            get
            {
                if (_description == null)
                {
                    _description = Get<string>("RazorBlog_Description", "Minimal & Fast Blogging for ASP.NET Core");
                }
                return _description;
            }
            set
            {
                Set("RazorBlog_Description", value);
                _description = value;
            }
        } 

        /// <summary>
        /// Gets/sets the URL prefix for the entire blog.
        /// </summary>
        public virtual string BlogPrefix 
        {
            get
            {
                if (_blogPrefix == null)
                {
                    _blogPrefix = Get<string>("RazorBlog_BlogPrefix", "/");
                }
                return _blogPrefix;
            }
            set
            {
                Set("RazorBlog_BlogPrefix", value);
                _blogPrefix = value;
            }
        }

        /// <summary>
        /// Gets/sets the URL slug for the blog archive.
        /// </summary>
        public virtual string ArchiveSlug
        {
            get
            {
                if (_archiveSlug == null)
                {
                    _archiveSlug = Get<string>("RazorBlog_ArchiveSlug", "/blog");
                }
                return _archiveSlug;
            }
            set
            {
                Set("RazorBlog_ArchiveSlug", value);
                _archiveSlug = value;
            }
        }

        /// <summary>
        /// Gets/sets the page size for the post listing.
        /// </summary>
        public virtual int PageSize
        {
            get
            {
                if (!_pageSize.HasValue)
                {
                    _pageSize = Get<int>("RazorBlog_PageSize", 5);
                }
                return _pageSize.Value;
            }
            set
            {
                Set("RazorBlog_PageSize", value);
                _pageSize = value;
            }
        }

        /// <summary>
        /// Gets/sets the currently active theme.
        /// </summary>
        public virtual string Theme
        {
            get
            {
                if (_theme == null)
                {
                    _theme = Get<string>("RazorBlog_Theme", "Persona");
                }
                return _theme;
            }
            set
            {
                Set("RazorBlog_Theme", value);
                _theme = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public Settings(Db db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets the value of the setting with the given key,
        /// or the default value if the setting is missing.
        /// </summary>
        /// <param name="key">The unique key</param>
        /// <param name="defaultValue">The default value</param>
        /// <typeparam name="T">The value type</typeparam>
        /// <returns>The setting value</returns>
        protected virtual T Get<T>(string key, T defaultValue)
        {
            var setting = _db.Settings
                .FirstOrDefault(s => s.Key == key);

            if (setting == null)
            {
                setting = new Setting
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Value = JsonConvert.SerializeObject(defaultValue)
                };
                _db.Settings.Add(setting);
                _db.SaveChanges();

                return defaultValue;
            }

            if (setting.Value != null)
            {
                return JsonConvert.DeserializeObject<T>(setting.Value);
            }
            return default(T);
        }

        /// <summary>
        /// Sets the value of the setting with the given key.
        /// </summary>
        /// <param name="key">The unique key</param>
        /// <param name="value">The value</param>
        protected virtual void Set(string key, object value)
        {
            var setting = _db.Settings
                .FirstOrDefault(s => s.Key == key);

            if (setting == null)
            {
                setting = new Setting
                {
                    Id = Guid.NewGuid(),
                    Key = key
                };
                _db.Settings.Add(setting);
            }
            setting.Value = value != null ? JsonConvert.SerializeObject(value) : null;

            _db.SaveChanges();
        }
    }
}