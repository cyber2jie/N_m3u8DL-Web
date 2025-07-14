
using NLog;

namespace N_m3u8DL_Web.Router
{
    public abstract class AbsRouter : IRouter
    {

        protected static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected List<RouteMap> routes { get; set; } = new List<RouteMap>();

        public abstract void BeforeMount();
        public abstract string Group();

        public List<RouteMap> RouteMap()
        {
            return routes;
        }

        protected void AddRoute(RouteType type, string path, RequestDelegate reqDelegate)
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
