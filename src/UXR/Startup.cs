using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UXR.Startup))]
namespace UXR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
