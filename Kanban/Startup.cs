using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kanban.Startup))]
namespace Kanban
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
