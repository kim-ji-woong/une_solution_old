using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace InternalUnE
{
    using Data;

    public class Startup
    {
        public static string SiteURL = "";
        public static List<LinkData> Links = new List<LinkData>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie();

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.
                AuthenticationScheme).AddCookie(options =>
                {
                    options.Cookie.Path = "";
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
                });

            /*services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = System.TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Forbidden/";
                });*/

            // 세션 사용
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromSeconds(10);
            });
            ////////////////////////////////////////////////////

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            SiteURL = Configuration["AppConfig:Common:url"];
            ReadLink();
        }

        private void ReadLink()
        {
            string strLinks = Configuration["AppConfig:Link"];

            if (strLinks == null || strLinks.Length == 0)
                return;

            string[] tokens = strLinks.Split('[');

            foreach (string strToken in tokens)
            {
                int index = strToken.LastIndexOf(']');

                if (index < 0)
                    continue;

                string strData = strToken.Substring(0, index);
                string[] datas = strData.Split(',');

                if (datas.Length != 4)
                    continue;

                string strUrl = datas[0].Trim();
                string strIconType = datas[1].Trim();

                LinkData link = LinkData.ParseData(datas[0].Trim(), datas[1].Trim(), datas[2].Trim(), datas[3].Trim());

                if (link != null)
                    Links.Add(link);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            /*app.UseProtectFolder(new ProtectFolderOptions
            {
                Path = "/static/media/secret",
                PolicyName = "Authenticated"
            });*/

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
