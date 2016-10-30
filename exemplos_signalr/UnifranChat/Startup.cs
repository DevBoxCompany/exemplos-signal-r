using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UnifranChat.Startup))]
namespace UnifranChat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            //app.MapHubs();
            ConfigureAuth(app);
        }
    }
}
