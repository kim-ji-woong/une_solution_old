using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dnsDBUtil;

namespace WifiSensorService
{
    using Data;

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
            services.AddCors(o => o.AddPolicy("UnEPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddControllers();

            string strDBName = Configuration["Site:DBName"];
            string strDBType = Configuration["Site:DBType"];
            string strWebServerURL = Configuration["Site:WebServerURL"];

            string strLifeTime = Configuration["Data:lifeTime"];

            string strRebootMinutes = Configuration["Data:rebootMinutes"];
            string strWarmingupMinutes = Configuration["Data:warmingupMinutes"];

            int nDBType, nLifeTime;
            int nRebootMinutes, nWarmingupMinutes;

            if (int.TryParse(strDBType.Trim(), out nDBType) && int.TryParse(strLifeTime.Trim(), out nLifeTime) && int.TryParse(strRebootMinutes.Trim(), out nRebootMinutes) && int.TryParse(strWarmingupMinutes.Trim(), out nWarmingupMinutes))
            {
                services.AddTransient(service => new Option(new WebDBManager(strDBName, nDBType, 1, strWebServerURL), nLifeTime, nRebootMinutes, nWarmingupMinutes));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
