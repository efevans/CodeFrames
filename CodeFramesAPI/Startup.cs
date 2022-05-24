using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFrames;
using CodeFramesAPI.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace CodeFramesAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSignalR()
                .AddJsonProtocol(
                    options => options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver()
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            var cache = app.ApplicationServices.GetService<IMemoryCache>();
            //Game game = new Game(new DanbooruImageGetter());
            Game game = new Game(new DefaultImagesImageGetter());
            cache.Set("CodeFrames", game, new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSignalR(routes =>
            {
                routes.MapHub<GameHub>("/gamehub");
            });
        }
    }
}
