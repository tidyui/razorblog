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
    public class GravatarTests
    {
        private readonly BlogService _blog;

        public GravatarTests()
        {
            // Create db context
            var builder = new DbContextOptionsBuilder<Db>();
            builder.UseSqlite("Filename=./razorblog.tests.db");
            var db = new Db(builder.Options);

            // Create service
            _blog = new BlogService(db);
        }

        [Fact]
        public void GetGravatar()
        {
            var url = _blog.GetGravatar("hakan@tidyui.com");

            Assert.Equal("https://www.gravatar.com/avatar/822bb9b48b7ecc98c5be44a74369a224?s=60&d=blank", url);
        }

        [Fact]
        public void GetGravatarWithSize()
        {
            var url = _blog.GetGravatar("hakan@tidyui.com", 120);

            Assert.Equal("https://www.gravatar.com/avatar/822bb9b48b7ecc98c5be44a74369a224?s=120&d=blank", url);
        }
    }
}
