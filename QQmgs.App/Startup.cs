using Microsoft.Owin;

using Twitter.App;
using Twitter.Data.UnitOfWork;

[assembly: OwinStartup(typeof(Startup))]

namespace Twitter.App
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}