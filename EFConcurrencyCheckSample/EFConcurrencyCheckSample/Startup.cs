using System.Web.Http;
using Owin;

namespace EFConcurrencyCheckSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();
            httpConfig.Routes.MapHttpRoute("DefaultRoute", "api/{controller}");
            app.UseWebApi(httpConfig);
        }
    }
}