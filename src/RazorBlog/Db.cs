/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/razorblog
 *
 */

using System;
using Microsoft.EntityFrameworkCore;
using RazorBlog.Models;

namespace RazorBlog
{
    public sealed class Db : DbContext
    {
        private static bool _isInitialized;
        private static object _mutex = new Object();

        /// <summary>
        /// Gets/sets the category set.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets/sets the comment set.
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Gets/sets the post set.
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// Gets/sets the setting set.
        /// </summary>
        public DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// Gets/sets the tag set.
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">The db options</param>
        public Db(DbContextOptions<Db> options) : base(options)
        {
            if (!_isInitialized)
            {
                lock (_mutex)
                {
                    if (!_isInitialized)
                    {
                        // Migrate database
                        Database.Migrate();

                        _isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Configures and creates the database model
        /// </summary>
        /// <param name="mb">The model builder</param>
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Category>().ToTable("RazorBlog_Categories");
            mb.Entity<Category>().Property(c => c.Title).HasMaxLength(64).IsRequired();
            mb.Entity<Category>().Property(c => c.Slug).HasMaxLength(64).IsRequired();
            mb.Entity<Category>().HasIndex(c => c.Slug).IsUnique();

            mb.Entity<Comment>().ToTable("RazorBlog_Comments");
            mb.Entity<Comment>().Property(c => c.AuthorName).HasMaxLength(128).IsRequired();
            mb.Entity<Comment>().Property(c => c.AuthorEmail).HasMaxLength(128).IsRequired();

            mb.Entity<Post>().ToTable("RazorBlog_Posts");
            mb.Entity<Post>().Property(p => p.Title).HasMaxLength(128).IsRequired();
            mb.Entity<Post>().Property(p => p.Slug).HasMaxLength(128).IsRequired();
            mb.Entity<Post>().Property(p => p.MetaKeywords).HasMaxLength(128);
            mb.Entity<Post>().Property(p => p.MetaDescription).HasMaxLength(255);
            mb.Entity<Post>().OwnsOne(p => p.Author, md => md.Property(m => m.Name).HasColumnName("AuthorName"));
            mb.Entity<Post>().OwnsOne(p => p.Author, md => md.Property(m => m.Email).HasColumnName("AuthorEmail"));
            mb.Entity<Post>().OwnsOne(p => p.Excerpt, md => md.Property(m => m.Value).HasColumnName("Excerpt"));
            mb.Entity<Post>().OwnsOne(p => p.Body, md => md.Property(m => m.Value).HasColumnName("Body"));
            mb.Entity<Post>().HasIndex(p => p.Slug).IsUnique();
            mb.Entity<Post>().Ignore(p => p.CommentCount);

            mb.Entity<Setting>().ToTable("RazorBlog_Settings");
            mb.Entity<Setting>().Property(s => s.Key).HasMaxLength(64).IsRequired();
            mb.Entity<Setting>().HasIndex(s => s.Key).IsUnique();

            mb.Entity<Tag>().ToTable("RazorBlog_Tags");
            mb.Entity<Tag>().Property(t => t.Title).HasMaxLength(64).IsRequired();
            mb.Entity<Tag>().Property(t => t.Slug).HasMaxLength(64).IsRequired();
            mb.Entity<Tag>().HasIndex(t => new { t.PostId, t.Slug }).IsUnique();
        }
    }
}