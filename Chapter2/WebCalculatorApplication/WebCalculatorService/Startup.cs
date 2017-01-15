namespace WebCalculatorService
{
    using App_Start;
    using Owin;
    using System.Web.Http;
    public class Startup : IOwinAppBuilder
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            FormatterConfig.ConfigureFormatters(config.Formatters);
            RouteConfig.RegisterRoutes(config.Routes);
            appBuilder.UseWebApi(config);
        }
    }
}