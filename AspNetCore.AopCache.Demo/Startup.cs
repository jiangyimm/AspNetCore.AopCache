using AspectCore.Extensions.DependencyInjection;
using AspNetCore.AopCache.CacheService;
using AspNetCore.AopCache.Demo.Bll;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.AopCache.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITestService, TestService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Use Redis Cache
            services.AddRedisCache(config =>
            {
                config.Expiration = 20;
                config.RedisHost = "localhost:32768";
            });

            //Use Memory Cache
            //services.AddMemoryCache(config => { config.Expiration = 20; });

            //Use Custom Cache
            //services.AddCustomCache<MyCacheService>();
            //services.AddCustomCache<OverrideCacheService>(config => { config.Expiration = 30; });

            return services.ConfigureDynamicProxy(p =>
            {
                p.ThrowAspectException = false;
            }).BuildDynamicProxyServiceProvider();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
