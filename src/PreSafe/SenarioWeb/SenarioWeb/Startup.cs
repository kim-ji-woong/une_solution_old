using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SenarioWeb.Startup))]
namespace SenarioWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
