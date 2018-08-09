using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorBlog.Models;
using RazorBlog.Services;

namespace RazorTemplate
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorBlog(options => options.UseSqlite("Filename=./razorblog.db"))
                .WithDependencies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IBlog blog)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRazorBlog(blog)
                .WithDependencies()
                .WithFileCache(7)
                .Run();

            // Seed
            if ((await blog.Api.GetArchive()).Items.Length == 0)
            {
                var post = new Post
                {
                    Title = "Mattis Ullamcorper Parturient",
                    Category = "Ornare Cursus",
                    MetaKeywords = "Pharetra, Mattis, Sem, Venenatis",
                    MetaDescription = "Aenean eu leo quam. Pellentesque ornare sem lacinia quam venenatis vestibulum.",
                    Excerpt = "Donec ullamcorper nulla non metus auctor fringilla. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus.",
                    Body = "Cras justo odio, **dapibus** ac facilisis in, egestas eget quam. Vestibulum id ligula porta felis euismod semper. Donec sed odio dui. Nullam id dolor id nibh ultricies vehicula ut id elit. Donec sed odio dui.\n\nCum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus sagittis lacus vel augue laoreet rutrum faucibus dolor auctor. Donec sed odio dui. Cras justo odio, dapibus ac facilisis in, egestas eget quam. Sed posuere consectetur est at lobortis. Donec sed odio dui. Morbi leo risus, porta ac consectetur ac, vestibulum at eros.",
                    Author = new Author 
                    {
                        Name = "Håkan Edling",
                        Email = "hakan@tidyui.com"
                    },
                    Published = DateTime.Now
                };
                post.Tags.Add("Mollis Ipsum");
                post.Tags.Add("Cras Etiam");
                await blog.Api.SavePost(post);

                post = new Post
                {
                    Title = "Lorem Tellus Parturient",
                    Category = "Euismod Tortor",
                    MetaKeywords = "Consectetur, Inceptos, Purus, Ultricies",
                    MetaDescription = "Nulla vitae elit libero, a pharetra augue.",
                    Excerpt = "Maecenas sed diam eget risus varius blandit sit amet non magna. Sed posuere consectetur est at lobortis. Donec id elit non mi porta gravida at eget metus.",
                    Body = "Vivamus sagittis lacus vel augue laoreet rutrum faucibus dolor auctor. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. Integer posuere erat a ante venenatis dapibus posuere velit aliquet. Donec ullamcorper nulla non metus auctor fringilla. Cras mattis consectetur purus sit amet fermentum. Maecenas sed diam eget risus varius blandit sit amet non magna.",
                    Author = new Author 
                    {
                        Name = "Håkan Edling",
                        Email = "hakan@tidyui.com"
                    },
                    Published = DateTime.Now
                };
                post.Tags.Add("Cras Etiam");
                post.Tags.Add("Venenatis Ipsum");
                await blog.Api.SavePost(post);

                var comment = new Comment
                {
                    PostId = post.Id,
                    AuthorEmail = "hakan@tidyui.com",
                    AuthorName = "Håkan Edling",
                    Body = "Maecenas sed diam eget risus varius blandit sit amet non magna. Cras justo odio, dapibus ac facilisis in, egestas eget quam.",
                    IsApproved = true,
                    Published = DateTime.Now
                };
                await blog.Api.SaveComment(comment);
            }
        }
    }
}
