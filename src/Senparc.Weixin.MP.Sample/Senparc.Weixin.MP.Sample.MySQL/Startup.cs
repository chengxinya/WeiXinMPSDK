using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Senparc.Weixin.MP.Sample.MySQL.Models;

namespace Senparc.Weixin.MP.Sample.MySQL
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkMySql()
                .AddDbContext<SenparcContext>(x => x.UseMySql("Server=senparcsdk.mysqldb.chinacloudapi.cn;Port=3306;Database=test;Uid=senparcsdk%mysql;Pwd=!@#EWQASD123;Connection Reset=false"));

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Init the mysql database
            app.ApplicationServices.GetRequiredService<SenparcContext>().Database.Migrate();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
