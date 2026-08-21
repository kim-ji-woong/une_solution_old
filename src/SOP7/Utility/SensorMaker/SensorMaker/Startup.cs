using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace SensorMaker
{
    using Service;

    public class Startup
    {
        public static string ResourceRootPath = "";
        public static string TempResourceRootPath = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 세션 사용
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromSeconds(10);
            });

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            string strSiteID = Configuration["AppConfig:Site:ID"];
            string strDBName = Configuration["AppConfig:Site:DBName"];
            string strDBType = Configuration["AppConfig:Site:DBType"];
            string strWebServerURL = Configuration["AppConfig:Site:WebServerURL"];

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID.Trim(), out nSiteID) && int.TryParse(strDBType.Trim(), out nDBType))
            {
                services.AddTransient<global::TeamEditor.IDAL.IDataManager>(service => new global::TeamEditor.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::Common.IDAL.IDataManager>(service => new global::Common.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::SOPManager.IDAL.IDataManager>(service => new global::SOPManager.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::SDMS.IDAL.IDataManager>(service => new global::SDMS.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
            }

            string strSystemMail = Configuration["AppConfig:Account:SystemMail"];
            string strAdminMail = Configuration["AppConfig:Account:AdminMail"];
            string strSystemCode = Configuration["AppConfig:Account:SystemCode"];
            string strSiteURL = Configuration["AppConfig:Common:url"];
            string strSolutionName = Configuration["AppConfig:Common:solutionName"];
            string strExternalLogin = Configuration["AppConfig:Common:externalLogin"];

            if (strSystemMail != null && strSystemMail.Trim().Length > 0 &&
                strAdminMail != null && strAdminMail.Trim().Length > 0 &&
                strSystemCode != null && strSystemCode.Trim().Length > 0 &&
                strSiteURL != null && strSiteURL.Trim().Length > 0 &&
                strSolutionName != null && strSolutionName.Trim().Length > 0)
            {
                services.AddTransient(service => new OptionManager(strSystemMail, strAdminMail, strSystemCode, strSiteURL, strSolutionName, strWebServerURL, strExternalLogin));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Set up custom content types -associating file extension to MIME type
            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".glb"] = "model/gltf+binary";
            provider.Mappings[".gltf"] = "model/gltf+json";

            app.UseStaticFiles();

            // 고용량 버전 모델링 파일 설정
            string strPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp\\build\\resource");

            if (Directory.Exists(strPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(strPath),
                    RequestPath = "/resource",
                    ContentTypeProvider = provider
                });
            }

            if (env.IsDevelopment())
            {
                ResourceRootPath = "ClientApp\\public\\resource\\UserData\\Regular";
                TempResourceRootPath = "ClientApp\\..\\bin\\resource\\UserData\\Temporary";
                app.UseDeveloperExceptionPage();
            }
            else
            {
                ResourceRootPath = "ClientApp\\build\\resource\\UserData\\Regular";
                TempResourceRootPath = "ClientApp\\build\\resource\\UserData\\Temporary";
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                // Areas 경로
                endpoints.MapControllerRoute(
                    name: "SensorMaker",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

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
