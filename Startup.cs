using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Core {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            //services.AddMvc();
            //Add service for accessing current HttpContext AND ActionContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor> ();
            services.AddSingleton<Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor, Microsoft.AspNetCore.Mvc.Infrastructure.ActionContextAccessor> ();
            services.Configure<CookiePolicyOptions> (options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //Add service for accessing current HttpContext
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Add framework services.
            // services.AddMvc (
            //     config => {
            //         config.Filters.Add (new MiddlewareFilters.Auth ());
            //     });
            //Filters Auth
            //services.AddScoped<MiddlewareFilters.Auth> ();
            //Service Context
            //services.AddDbContext<TMShopsCore.Models.TMShopsContext> (options => options.UseSqlServer (Configuration.GetConnectionString ("MainConnection"), b => b.UseRowNumberForPaging ()));
            services.AddDistributedMemoryCache (); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession ();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            //Add service for accessing current HttpContext AND ActionContext
            TM.Core.Helper.TMAppContext.Services = app.ApplicationServices;
            //CultureInfo Defalut
            TM.Core.Format.Formating.CultureInfo();
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Home/Error");
                app.UseHsts ();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles ();
            app.UseSession();
            app.UseCookiePolicy ();

            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}