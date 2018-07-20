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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using RazorBlog.Models;

namespace RazorBlog.Services
{
    public class BlogService : IBlogService
    {
        protected readonly Db _db;
        protected static IMapper _mapper;
        protected static object _mutex = new Object();
        protected static bool _isInitialized = false;
        protected static BlogSettings _settings = new BlogSettings();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public BlogService(Db db)
        {
            _db = db;

            if (!_isInitialized)
            {
                Init();
            }
        }

        /// <summary>
        /// Gets the current blog settings.
        /// </summary>
        public BlogSettings Settings => _settings;

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
        /// Gets the post with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post</returns>
        public virtual async Task<Post> GetPostBySlug(string slug)
        {
            var post = await GetQuery()
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (post != null)
                post.CommentCount = await _db.Comments.CountAsync(c => c.PostId == post.Id && c.IsApproved);
            return post;
        }

        /// <summary>
        /// Gets the post archive.
        /// </summary>
        /// <param name="page">The current page of the archive</param>
        /// <param name="category">The optional category slug</param>
        /// <param name="tag">The optional tag slug</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <returns>The archive</returns>
        public virtual async Task<Post[]> GetArchive(int page = 0, string category = null, string tag = null, int? year = null, int? month = null)
        {
            var query = GetQuery();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.Slug == category);
            if (!string.IsNullOrEmpty(tag))
                query = query.Where(p => p.Tags.Any(t => t.Slug == tag));
            
            if (year.HasValue)
            {
                var from = new DateTime(year.Value, month.HasValue ? month.Value : 1, 1);
                var to = month.HasValue ? from.AddMonths(1) : from.AddYears(1);

                query = query.Where(p => p.Published >= from && p.Published < to);
            }
            else
            {
                query = query.Where(p => p.Published <= DateTime.Now);
            }

            var posts = await query
                .OrderByDescending(p => p.Published)
                .Skip(page * Settings.PageSize)
                .Take(Settings.PageSize)
                .ToArrayAsync();

            foreach (var post in posts)
            {
                post.CommentCount = await _db.Comments.CountAsync(c => c.PostId == post.Id && c.IsApproved);
                post.Tags = post.Tags.OrderBy(t => t.Title).ToList();
            }
            return posts;
        }

        /// <summary>
        /// Saves the given post.
        /// </summary>
        /// <param name="model">The post to save</param>
        /// <returns>The id of the post</returns>
        public async Task<Guid> SavePost(Post model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (string.IsNullOrEmpty(model.Slug))
                model.Slug = GenerateSlug(model.Title);

            var post = await GetQuery(withTracking: true)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (post == null)
            {
                post = new Post
                {
                    Id = model.Id
                };
                _db.Posts.Add(post);
            }

            _mapper.Map<Post, Post>(model, post);

            if (model.Category != null)
            {
                if (!_db.Categories.Any(c => c.Id == model.Category.Id))
                {
                    post.CategoryId = model.Category.Id = Guid.NewGuid();
                    model.Category.Slug = GenerateSlug(model.Category.Title);
                    
                    await _db.Categories.AddAsync(model.Category);
                }
            }

            // Delete removed tags
            var currentTags = model.Tags.Select(t => t.Id).ToArray();
            var removed = post.Tags.Where(t => currentTags.Contains(t.Id)).ToArray();
            foreach (var tag in removed)
            {
                post.Tags.Remove(tag);
            }

            // Add new tags
            foreach (var tag in model.Tags)
            {
                if (!post.Tags.Any(t => t.Id == tag.Id))
                {
                    tag.Id = Guid.NewGuid();
                    tag.PostId = post.Id;
                    tag.Slug = GenerateSlug(tag.Title);
                    
                    post.Tags.Add(tag);
                }
            }
            post.LastModified = DateTime.Now;

            await _db.SaveChangesAsync();

            return model.Id;
        }

        /// <summary>
        /// Gets the comments for the post with the specified id.
        /// </summary>
        /// <param name="postId">The post id</param>
        /// <param name="page">The current page of the comments</param>
        /// <returns>The available comments</returns>
        public Task<Comment[]> GetComments(Guid postId, int page = 0)
        {
            return _db.Comments
                .Where(c => c.PostId == postId && c.IsApproved)
                .OrderByDescending(c => c.Published)
                .Skip(page * Settings.PageSize)
                .Take(Settings.PageSize)
                .ToArrayAsync();
        }

        /// <summary>
        /// Saves the given comment.
        /// </summary>
        /// <param name="model">The comment to save</param>
        /// <returns>The id of the comment</returns>
        public async Task<Guid> SaveComment(Comment model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (string.IsNullOrEmpty(model.AuthorName) || model.AuthorName.Length > 128)
                throw new ArgumentException("Author name is required and has a max length of 128 characters.");
            if (string.IsNullOrEmpty(model.AuthorEmail) || model.AuthorEmail.Length > 128)
                throw new ArgumentException("Author email is required and has a max length of 128 characters.");

            var comment = await _db.Comments
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (comment == null)
            {
                comment = new Comment
                {
                    Id = model.Id,
                    PostId = model.PostId
                };
                await _db.Comments.AddAsync(comment);
            }

            _mapper.Map<Comment, Comment>(model, comment);

            await _db.SaveChangesAsync();

            return comment.Id;
        }

        /// <summary>
        /// Generates a new slug from the given string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The slug</returns>
        public virtual string GenerateSlug(string str) 
        {
            // Trim & make lower case
            var slug = str.Trim().ToLower();

            // Convert culture specific characters
            slug = slug
                .Replace("å", "a")
                .Replace("ä", "a")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ö", "o")
                .Replace("ó", "o")
                .Replace("ò", "o")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("í", "i")
                .Replace("ì", "i");

            // Remove special characters
            slug = Regex.Replace(slug, @"[^a-z0-9-/ ]", "").Replace("--", "-");

            // Remove whitespaces
            slug = Regex.Replace(slug.Replace("-", " "), @"\s+", " ").Replace(" ", "-");

            // Remove slashes
            slug = slug.Replace("/", "-");

            // Remove multiple dashes
            slug = Regex.Replace(slug, @"[-]+", "-");

            // Remove leading & trailing dashes
            if (slug.EndsWith("-"))
                slug = slug.Substring(0, slug.LastIndexOf("-"));
            if (slug.StartsWith("-"))
                slug = slug.Substring(Math.Min(slug.IndexOf("-") + 1, slug.Length));
            return slug;
        }

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

        /// <summary>
        /// Gets the post base query.
        /// </summary>
        /// <param name="withTracking">If change tracking should be enabled</param>
        /// <returns>The query</returns>
        protected virtual IQueryable<Post> GetQuery(bool withTracking = false)
        {
            IQueryable<Post> query = _db.Posts;

            if (!withTracking)
                query = query.AsNoTracking();

            return query
                .Include(p => p.Category)
                .Include(p => p.Tags);
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        protected void Init()
        {
            lock (_mutex)
            {
                if (!_isInitialized)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Models.Comment, Models.Comment>()
                            .ForMember(c => c.Post, o => o.Ignore());
                        cfg.CreateMap<Models.Post, Models.Post>()
                            .ForMember(p => p.Id, o => o.Ignore())
                            .ForMember(p => p.Category, o => o.Ignore())
                            .ForMember(p => p.Tags, o => o.Ignore());
                    });
                    config.AssertConfigurationIsValid();
                    _mapper = config.CreateMapper();

                    _isInitialized = true;
                }
            }
        }
    }
}