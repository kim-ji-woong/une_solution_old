using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SensorSimulationWeb.Startup))]
namespace SensorSimulationWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
