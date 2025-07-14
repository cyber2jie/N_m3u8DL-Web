using N_m3u8DL_RE.Export.Util;
using N_m3u8DL_Web.Util;
using NLog;

namespace N_m3u8DL_Web.Router
{
    internal class Index
    {

        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Register(WebApplication app)
        {

            RegiesterByType<M3u8>(app);

            RegiesterByType<Ts>(app);

        }


        private static void RegiesterByType<T>(WebApplication app) where T : IRouter
        {
            Type type = typeof(T);

            IRouter instance = (T)Activator.CreateInstance(type);
            instance.BeforeMount();

            string group = instance.Group();
            RouteGroupBuilder rgb = app.MapGroup(group);

            foreach (var routeMap in instance.RouteMap())
            {
                RouteType routeType = routeMap.RT;
                string path = routeMap.Path;
                RequestDelegate reqDelegate = routeMap.ReqDelegate;

                instance.OnMount(rgb, routeMap);

                switch (routeType)
                {
                    case RouteType.GET:
                        rgb.MapGet(path, reqDelegate);
                        break;
                    case RouteType.POST:
                        rgb.MapPost(path, reqDelegate);
                        break;
                }

            }


            instance.AfterMount();

        }
    }

}
