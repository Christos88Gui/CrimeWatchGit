using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CrimeWatch.Startup))]
namespace CrimeWatch
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
