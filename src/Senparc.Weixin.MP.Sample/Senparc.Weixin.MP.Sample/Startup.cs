using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Senparc.Weixin.Entities;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Senparc.Weixin.MP.Sample.MySQL.Models;

namespace Senparc.Weixin.MP.Sample
{
    public class Startup
    {
        public static SenparcWeixinSetting SenparcWeixin;
        public static string RootPath;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            RootPath = env.ContentRootPath;

            //Senparc.Weixin.SDK 配置
            builder.AddJsonFile("SenparcWeixin.json", optional: true);
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //Senparc.Weixin.MP.Sample.CommonService需要使用到
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<SenparcWeixinSetting>(Configuration.GetSection("SenparcWeixinSetting"));
            services.AddEntityFrameworkMySql()
                .AddDbContext<SenparcContext>(x => x.UseMySql("server=localhost;uid=root;pwd=root;database=senparc"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<SenparcWeixinSetting> senparcWeixinSetting, SenparcContext senparcContent)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //迁移数据库
            //app.ApplicationServices.GetRequiredService<SenparcContext>().Database.Migrate();

            //Senparc.Weixin SDK 配置
            SenparcWeixin = senparcWeixinSetting.Value;
            //app.UseMiddleware()
        }
    }
}
