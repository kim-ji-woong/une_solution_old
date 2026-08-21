using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartEyeWeb.Startup))]
namespace SmartEyeWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
