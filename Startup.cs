using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SpaServices.Webpack;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TM.Core.Helper.Extensions;

namespace Billing.Core {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            //Add service for accessing current HttpContext AND ActionContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // services.AddHttpContextAccessor();

            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDataProtection()
                .PersistKeysToFileSystem(GetKeyRingDirInfo())
                .SetApplicationName("SharedCookieApp");

            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = ".AspNet.SharedCookie";
            });
            //Filters Auth
            services.AddScoped<MiddlewareFilters.Auth>();
            //Service Context
            //services.AddDbContext<TMShopsCore.Models.TMShopsContext> (options => options.UseSqlServer (Configuration.GetConnectionString ("MainConnection"), b => b.UseRowNumberForPaging ()));
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            // services.AddSession(options => {
            //     options.IdleTimeout = TimeSpan.FromDays(15);
            //     options.Cookie.HttpOnly = true;
            // });
            // Add framework services.
            services.AddMvc(config => { config.Filters.Add(new MiddlewareFilters.Auth()); })
                .AddSessionStateTempDataProvider()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // var sp = services.BuildServiceProvider();
            // var service = sp.GetService<IHttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            //Add service for accessing current HttpContext AND ActionContext
            TM.Core.HttpContext.Current.Services = app.ApplicationServices;
            //TM.Core.Helper.TMAppContext.HttpHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            //CultureInfo Defalut
            TM.Core.Format.Formating.CultureInfo();
            //Logger
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                // app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                // {
                //     HotModuleReplacement = true
                // });
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseCookiePolicy();

            // Add MVC to the request pipeline.
            // app.UseCors(builder =>
            //     builder.AllowAnyOrigin()
            //     .AllowAnyHeader()
            //     .AllowAnyMethod());
            // UseExceptionHandler
            // app.UseExceptionHandler(
            //     builder => {
            //         builder.Run(
            //             async context => {
            //                 context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            //                 context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            //                 var error = context.Features.Get<IExceptionHandlerFeature>();
            //                 if (error != null) {
            //                     context.Response.AddApplicationError(error.Error.Message);
            //                     await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
            //                 }
            //             });
            //     });
            // 
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private System.IO.DirectoryInfo GetKeyRingDirInfo() {
            var startupAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var applicationBasePath = System.AppContext.BaseDirectory;
            var directoryInfo = new System.IO.DirectoryInfo(applicationBasePath);
            do {
                directoryInfo = directoryInfo.Parent;

                var keyRingDirectoryInfo = new System.IO.DirectoryInfo(System.IO.Path.Combine(directoryInfo.FullName, "KeyRing"));
                if (keyRingDirectoryInfo.Exists) {
                    return keyRingDirectoryInfo;
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"KeyRing folder could not be located using the application root {applicationBasePath}.");
        }
    }
}