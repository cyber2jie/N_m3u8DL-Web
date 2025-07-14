namespace N_m3u8DL_Web.Router
{

    public enum RouteType
    {
        GET,
        POST
    }
  public class RouteMap
    {
        public RouteType RT { get; set; }
        public string Path { get; set; }
        public RequestDelegate ReqDelegate { get; set; }

    }
}
