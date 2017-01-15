using System.Web.Http;
using Owin;

namespace WebApi1
{
    public static class Startup
    {
        // このコードは Web API を構成します。スタートアップ クラスは、
        // WebApp.Start メソッドの型パラメーターとして指定されます。
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // セルフ ホストの Web API を構成します。
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
