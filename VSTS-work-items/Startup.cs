using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VSTS_work_items.Startup))]
namespace VSTS_work_items
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
