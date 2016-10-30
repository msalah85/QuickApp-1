﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickApp;
using QuickApp.MongoDb;
using QuickApp.Services.Interceptors;
using QuickApp.Web.Mvc;

namespace QuickAppWebTest
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services
                .AddQuickApp()
                .AddMongoService("mongodb://localhost:27017", "QuickAppTest", "mongodb")
                .AddInterceptor("mongodb", "InsertOne", Moment.Before, context =>
                {
                    context.Arguments.document.name += " 2";
                    context.Arguments.document.userId = 44;
                })
                .AddInterceptor("mongodb", "Find", Moment.After, context =>
                {
                    context.Result.Add(new {name = "From", surname = "After Find Interceptor"});
                })
                .AddInterceptor("mongodb", new MongoInterceptorImplementingInterface())
                .AddInterceptor("mongodb", new MongoInterceptorUsingMethodsNames());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
                routes.AddQuickAppRoute("qa");
            });

        }
    }
}