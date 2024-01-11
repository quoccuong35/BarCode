using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WMS.Startup))]
namespace WMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
