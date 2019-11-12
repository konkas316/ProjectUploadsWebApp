using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProjectUploads.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace ProjectUploads
{

    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
            
        }
        

        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddDbContextPool<AppDbContext>(
           options => options.UseSqlServer(_config.GetConnectionString("UploadDBConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });


            services.AddMvc().AddXmlSerializerFormatters(); 
            services.AddScoped<IUploadRepository, SQLUploadRepository>();

           
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            {
                
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
               
            }

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

        }

    }

    }

