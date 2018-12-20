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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using RazorBlog.Models;

namespace RazorBlog.Services
{
    public class Api : IApi
    {
        protected readonly Db _db;
        protected readonly ISettings _settings;
        protected readonly IMemoryCache _memCache;
        private static IMapper _mapper;
        private static object _mutex = new Object();
        private static bool _isInitialized = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="settings">The current settings</param>
        /// <param name="memCache">The optional cache</param>
        public Api(Db db, ISettings settings, IMemoryCache memCache = null)
        {
            _db = db;
            _settings = settings;
            _memCache = memCache;

            if (!_isInitialized)
            {
                Init();
            }
        }

        /// <summary>
        /// Gets the post with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post</returns>
        public virtual async Task<Post> GetPost(string slug)
        {
            var post = _memCache?.Get<Post>(slug);

            if (post == null)
            {
                post = await GetQuery()
                    .Where(p => p.Slug == slug)
                    .Select(p => new Post {
                        Id = p.Id,
                        CategoryId = p.CategoryId,
                        Title = p.Title,
                        Slug = p.Slug,
                        MetaKeywords = p.MetaKeywords,
                        MetaDescription = p.MetaDescription,
                        Excerpt = p.Excerpt,
                        Body = p.Body,
                        Author = new Author
                        {
                            Name = p.Author.Name,
                            Email = p.Author.Email
                        },
                        CommentCount = p.Comments.Count(c => c.IsApproved),
                        Published = p.Published,
                        LastModified = p.LastModified,
                        Category = p.Category,
                        Tags = p.Tags.OrderBy(t => t.Title).ToList()
                    }).FirstOrDefaultAsync();

                if (post != null)
                {
                    post.LastModified = post.LastModified.ToLocalTime();

                    if (post.Published.HasValue)
                    {
                        post.Published = post.Published.Value.ToLocalTime();
                    }
                    
                    _memCache?.Set(slug, post);
                }
            }
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
        public virtual async Task<PostList> GetArchive(int page = 1, string category = null, string tag = null, int? year = null, int? month = null)
        {
            var model = new PostList();
            var query = GetQuery();

            // Filter on category
            if (!string.IsNullOrEmpty(category))
            {
                model.Category = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Slug == category);

                if (model.Category != null)
                {
                    query = query.Where(p => p.CategoryId == model.Category.Id);
                }
            }

            // Filter on tag
            if (!string.IsNullOrEmpty(tag))
            {
                model.Tag = await _db.Tags
                    .FirstOrDefaultAsync(t => t.Slug == tag);

                if (model.Tag != null)
                {
                    query = query.Where(p => p.Tags.Any(t => t.Slug == tag));
                }
            }
            
            // Filter on period
            var now = DateTime.Now;
            if (year.HasValue && year.Value <= now.Year)
            {
                model.Year = year;
                model.Month = month;

                var from = new DateTime(year.Value, month.HasValue ? month.Value : 1, 1);
                var to = month.HasValue ? from.AddMonths(1) : from.AddYears(1);

                query = query.Where(p => p.Published >= from && p.Published < to);
            }
            else
            {
                query = query.Where(p => p.Published <= DateTime.Now.ToUniversalTime());
            }

            // Get total posts matching the query
            var count = await query.CountAsync();

            // Set page count and validate requested page
            model.PageCount = Math.Max(Convert.ToInt32(Math.Ceiling((double)count / _settings.PageSize)), 1);

            var currentPage = page;
            if (currentPage > model.PageCount)
            {
                currentPage = model.PageCount;
            }
            model.Page = currentPage;

            // Setup pagination
            if (model.PageCount > 1)
            {
                var baseUrl = _settings.ArchiveSlug +
                    (model.Category != null ? $"/category/{model.Category.Slug}" : "") +
                    (model.Tag != null ? $"/tag/{model.Tag.Slug}" : "");

                if (model.Page > 1)
                {
                    model.Pagination.HasPrev = true;
                    model.Pagination.PrevLink = $"{baseUrl}/page/{Math.Max(model.Page - 1, 1)}";
                }
                if (model.Page < model.PageCount)
                {
                    model.Pagination.HasNext = true;
                    model.Pagination.NextLink = $"{baseUrl}/page/{Math.Min(model.Page + 1, model.PageCount)}";
                }
            }

            model.Items = await query
                .OrderByDescending(p => p.Published)
                .Skip((page - 1) * _settings.PageSize)
                .Take(_settings.PageSize)
                .Select(p => new Post {
                    Id = p.Id,
                    CategoryId = p.CategoryId,
                    Title = p.Title,
                    Slug = p.Slug,
                    MetaKeywords = p.MetaKeywords,
                    MetaDescription = p.MetaDescription,
                    Excerpt = p.Excerpt,
                    Body = p.Body,
                    Author = new Author
                    {
                        Name = p.Author.Name,
                        Email = p.Author.Email
                    },
                    CommentCount = p.Comments.Count(c => c.IsApproved),
                    Published = p.Published,
                    LastModified = p.LastModified,
                    Category = p.Category,
                    Tags = p.Tags.OrderBy(t => t.Title).ToList()
                }).ToArrayAsync();

            // Get comment count and order tags
            foreach (var post in model.Items)
            {
                post.LastModified = post.LastModified.ToLocalTime();
                post.Published = post.Published.Value.ToLocalTime();
            }
            return model;
        }

        /// <summary>
        /// Saves the given post.
        /// </summary>
        /// <param name="model">The post to save</param>
        /// <returns>The id of the post</returns>
        public async Task<Guid> SavePost(Post model)
        {
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            if (string.IsNullOrEmpty(model.Slug))
            {
                model.Slug = GenerateSlug(model.Title);
            }

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
                if (string.IsNullOrEmpty(model.Category.Slug))
                {
                    model.Category.Slug = GenerateSlug(model.Category.Title);
                }

                var category = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Slug == model.Category.Slug);

                if (category == null)
                {
                    post.CategoryId = model.Category.Id = Guid.NewGuid();
                    model.Category.Slug = GenerateSlug(model.Category.Title);
                    
                    await _db.Categories.AddAsync(model.Category);
                }
                else
                {
                    post.CategoryId = category.Id;
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
            post.LastModified = DateTime.Now.ToUniversalTime();
            if (post.Published.HasValue)
            {
                post.Published = post.Published.Value.ToUniversalTime();
            }

            await _db.SaveChangesAsync();

            _memCache?.Remove(post.Slug);

            return model.Id;
        }

        /// <summary>
        /// Gets the comments for the post with the specified id.
        /// </summary>
        /// <param name="postId">The post id</param>
        /// <param name="page">The current page of the comments</param>
        /// <returns>The available comments</returns>
        public async Task<Comment[]> GetComments(Guid postId, int page = 0)
        {
            var comments = await _db.Comments
                .Where(c => c.PostId == postId && c.IsApproved)
                .OrderByDescending(c => c.Published)
                .Skip(page * _settings.PageSize)
                .Take(_settings.PageSize)
                .ToArrayAsync();

            foreach (var comment in comments)
            {
                comment.Published = comment.Published.ToLocalTime();
            }
            return comments;
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
            {
                throw new ArgumentException("Author name is required and has a max length of 128 characters.");
            }
            if (string.IsNullOrEmpty(model.AuthorEmail) || model.AuthorEmail.Length > 128)
            {
                throw new ArgumentException("Author email is required and has a max length of 128 characters.");
            }

            var comment = await _db.Comments
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (comment == null)
            {
                model.IsApproved = true;
                model.Published = DateTime.Now;

                comment = new Comment
                {
                    Id = model.Id,
                    PostId = model.PostId
                };
                await _db.Comments.AddAsync(comment);

                if (_memCache != null)
                {
                    // Since this is a new comment we need to purge
                    // the related post from cache
                    var postSlug = await _db.Posts
                        .Where(p => p.Id == model.PostId)
                        .Select(p => p.Slug)
                        .FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(postSlug))
                    {
                        _memCache.Remove(postSlug);
                    }
                }
            }
            _mapper.Map<Comment, Comment>(model, comment);
            comment.Published = comment.Published.ToUniversalTime();

            // Execute hook, if available
            Hooks.Comment.OnSave?.Invoke(comment);

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
            {
                slug = slug.Substring(0, slug.LastIndexOf("-"));
            }
            if (slug.StartsWith("-"))
            {
                slug = slug.Substring(Math.Min(slug.IndexOf("-") + 1, slug.Length));
            }
            return slug;
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
            {
                query = query.AsNoTracking();
            }

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