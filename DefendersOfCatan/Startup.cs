using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DefendersOfCatan.Startup))]
namespace DefendersOfCatan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
