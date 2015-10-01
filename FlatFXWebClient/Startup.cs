using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FlatFXWebClient.Startup))]
namespace FlatFXWebClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
