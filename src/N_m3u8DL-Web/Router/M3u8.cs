
using N_m3u8DL_RE.Export.Util;
using N_m3u8DL_Web.Util;
using NLog;

namespace N_m3u8DL_Web.Router
{
    public class M3u8 : IRouter
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        List<RouteMap> routes { get; set; } = new List<RouteMap>();

        public  void BeforeMount()
        {
            
            AddRoute(RouteType.POST, "/list", async (HttpContext context) =>
            {
                var url = context.Request.Form["url"];

                logger.Info("request url,{}", url);
                context.Response.ContentType = "application/json";
                var m3u8Info = await ExtractUtil.ExtractFromUrl(url.ToString());

                await context.Response.WriteAsync(JSONUtil.ToJson(m3u8Info));

            });


            AddRoute(RouteType.POST, "/parseKey", async (HttpContext context) =>
            {
                var url = context.Request.Form["url"];

                logger.Info("request url,{}", url);
                context.Response.ContentType = "application/json";
                var encryptInfo = await ExtractUtil.ExtractEncryptInfoFromUrl(url.ToString(),3);

                await context.Response.WriteAsync(JSONUtil.ToJson(encryptInfo));

            });




        }

        public string Group()
        {
            return "/api/m3u8";
        }

        public List<RouteMap> RouteMap()
        {
            return routes;
        }

        private void AddRoute(RouteType type, string path, RequestDelegate reqDelegate)
        {
            routes.Add(new RouteMap()
            {
                RT = type,
                Path = path,
                ReqDelegate = reqDelegate
            });
        }
    

    }
}
