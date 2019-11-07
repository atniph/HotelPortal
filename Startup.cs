using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelPortal.Data;
using HotelPortal.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelPortal
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<HotelPortalContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HotelPortalContext")));

            services.AddIdentity<SiteUser, IdentityRole<int>>()
                .AddEntityFrameworkStores<HotelPortalContext>()
                .AddDefaultTokenProviders();

            //    services.AddIdentityCore<SiteUser>()
            //       .AddRoles<IdentityRole<int>>()
            //        .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<SiteUser, IdentityRole<int>>>()
            //        .AddEntityFrameworkStores<HotelPortalContext>()
            //        .AddDefaultTokenProviders()
            //        .AddDefaultUI();


            services.AddTransient<IPortalService, PortalService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });

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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Hotels}/{action=Index}/{id?}");
            });
        }
    }
}
