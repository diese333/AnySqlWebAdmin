﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Rewrite;


namespace AnySqlWebAdmin
{
    
    
    public class Startup
    {
        
        public IConfiguration Configuration { get; }
        
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        } // End Constructor 


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                // TODO: Add configurable proxy list
                // options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });

            services.AddSingleton(new SqlService());
            services.AddMvc();
            // services.AddHsts();
            
                
            services.AddMvc(mvcOptions =>
            {
                // mvcOptions.Filters.Add(new Microsoft.AspNetCore.Mvc.RequireHttpsAttribute());

                mvcOptions.ValueProviderFactories.Add(new JsonValueProviderFactory());
                mvcOptions.ValueProviderFactories.Add(new AnywhereValueProviderFactory());
            });

        } // End Sub ConfigureServices 


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor 
                | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });


            // app.UseAuthentication();


            // var options = new Microsoft.AspNetCore.Rewrite.RewriteOptions().AddRedirectToHttps(
            //     Microsoft.AspNetCore.Http.StatusCodes.Status301MovedPermanently, 44384);
            // app.UseRewriter(options);


            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new List<string>()
                {
                    "map.htm", "index.htm", "index.html", "slick.htm"
                }
            });


            // app.UseHsts(); // Microsoft.AspNetCore.HttpsPolicy.dll
            // app.UseHsts(h => h.MaxAge(days: 365).preload());

            // app.UseHsts();

            app.UseStaticFiles();
            // app.UseMiddleware<SqlMiddleware>();
            app.UseSqlMiddleware();
            app.UseDataFeedMiddleware();
            
            app.UseMvc();


            // if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            // app.UseStatusCodePages();
            app.UseErrorHandlingMiddleware();



            // Will use url configured in 
            // AnySqlWebAdmin\AnySqlWebAdmin\Properties\launchSettings.json 
            // "launchUrl": "api/values", if value is present 
            // type, cs, db, query
        } // End Sub Configure 


    } // End Class Startup 


} // End Namespace AnySqlWebAdmin 
