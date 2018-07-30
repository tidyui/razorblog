/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;
using RazorBlog;
using RazorBlog.Services;
using Xunit;

namespace RazorBlog.Tests.Service
{
    public class SlugTests
    {
        private readonly BlogService _blog;

        public SlugTests()
        {
            // Create db context
            var builder = new DbContextOptionsBuilder<Db>();
            builder.UseSqlite("Filename=./razorblog.tests.db");
            var db = new Db(builder.Options);

            // Create service
            _blog = new BlogService(db);
        }

        [Fact]
        public void ToLowerCase() 
        {
            Assert.Equal("mycamelcasestring", _blog.GenerateSlug("MyCamelCaseString"));
        }

        [Fact]
        public void Trim() 
        {
            Assert.Equal("trimmed", _blog.GenerateSlug(" trimmed  "));
        }

        [Fact]
        public void ReplaceWhitespace() 
        {
            Assert.Equal("no-whitespaces", _blog.GenerateSlug("no whitespaces"));
        }

        [Fact]
        public void ReplaceDoubleDashes() 
        {
            Assert.Equal("no-whitespaces", _blog.GenerateSlug("no - whitespaces"));
            Assert.Equal("no-whitespaces", _blog.GenerateSlug("no & whitespaces"));
        }

        [Fact]
        public void TrimDashes() 
        {
            Assert.Equal("trimmed", _blog.GenerateSlug("-trimmed-"));
        }

        [Fact]
        public void RemoveSwedishCharacters() 
        {
            Assert.Equal("aaoaao", _blog.GenerateSlug("åäöÅÄÖ"));
        }

        [Fact]
        public void RemoveHyphenCharacters() 
        {
            Assert.Equal("aaooeeiiaaooeeii", _blog.GenerateSlug("áàóòéèíìÁÀÓÒÉÈÍÌ"));
        }

        [Fact]
        public void RemoveSlashesl() {
            Assert.Equal("no-more-slashes", _blog.GenerateSlug("no/more / slashes"));
        }
    }
}
