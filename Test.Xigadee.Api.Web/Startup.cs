using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Test.Xigadee.Api.Web.Startup))]
namespace Test.Xigadee.Api.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
