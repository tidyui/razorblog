using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            services.AddRazorBlog(options => options.UseSqlite("Filename=./razorblog.db"));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IBlogService blog)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRazorBlog(blog);
            app.UseStaticFiles();
            app.UseMvc();

            // Seed
            if ((await blog.GetArchive()).Length == 0)
            {
                var post = new Post
                {
                    Title = "Mattis Ullamcorper Parturient",
                    Category = "Ornare Cursus",
                    MetaKeywords = "Pharetra, Mattis, Sem, Venenatis",
                    MetaDescription = "Aenean eu leo quam. Pellentesque ornare sem lacinia quam venenatis vestibulum.",
                    Body = "Cras justo odio, **dapibus** ac facilisis in, egestas eget quam. Vestibulum id ligula porta felis euismod semper. Donec sed odio dui. Nullam id dolor id nibh ultricies vehicula ut id elit. Donec sed odio dui.",
                    Published = DateTime.Now
                };
                post.Tags.Add("Mollis Ipsum");
                post.Tags.Add("Cras Etiam");
                await blog.SavePost(post);

                post = new Post
                {
                    Title = "Lorem Tellus Parturient",
                    Category = "Euismod Tortor",
                    MetaKeywords = "Consectetur, Inceptos, Purus, Ultricies",
                    MetaDescription = "Nulla vitae elit libero, a pharetra augue.",
                    Body = "Vivamus sagittis lacus vel augue laoreet rutrum faucibus dolor auctor. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. Integer posuere erat a ante venenatis dapibus posuere velit aliquet. Donec ullamcorper nulla non metus auctor fringilla. Cras mattis consectetur purus sit amet fermentum. Maecenas sed diam eget risus varius blandit sit amet non magna.",
                    Published = DateTime.Now
                };
                post.Tags.Add("Cras Etiam");
                post.Tags.Add("Venenatis Ipsum");
                await blog.SavePost(post);
            }
        }
    }
}
