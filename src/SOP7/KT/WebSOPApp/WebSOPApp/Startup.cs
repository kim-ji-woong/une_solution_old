using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebSOPApp
{
    public class Startup
    {
        public static string SOPWebServerURL = "";
        public static string StreamServerURL = "";
        public static string SiteID = "";
        public static string SmsUrl = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("UnEPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            string strSiteID = Configuration["Site:ID"];
            SiteID = strSiteID;
            string strDBName = Configuration["Site:DBName"];
            string strDBType = Configuration["Site:DBType"];
            string strWebServerURL = Configuration["Site:WebServerURL"];
            string strSOPWebServerURL = Configuration["Site:SOPWebServerURL"];
            SOPWebServerURL = strSOPWebServerURL;
            string strStreamServerURL = Configuration["Site:StreamServerURL"];
            StreamServerURL = strStreamServerURL;
            SmsUrl = Configuration["Site:SmsUrl"];

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID.Trim(), out nSiteID) && int.TryParse(strDBType.Trim(), out nDBType))
            {
                services.AddTransient<global::SOPSimulator.IDAL.IDataManager>(service => new global::SOPSimulator.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::SOPManager.IDAL.IDataManager>(service => new global::SOPManager.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::TeamEditor.IDAL.IDataManager>(service => new global::TeamEditor.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::Common.IDAL.IDataManager>(service => new global::Common.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::SDMS.IDAL.IDataManager>(service => new global::SDMS.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
                services.AddTransient<global::NipaSOP.IDAL.IDataManager>(service => new global::NipaSOP.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL));
            }

            string strLogFolder = Configuration["Logger:logFolder"];
            string strLogTag = Configuration["Logger:logTag"];

            NipaSOP.BLL.Logger.LogFolder = strLogFolder;
            NipaSOP.BLL.Logger.LogTag = strLogTag;
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

            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "WebSOPApp",
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
