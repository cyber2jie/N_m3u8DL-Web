namespace N_m3u8DL_Web.Router
{
    public interface IRouter
    {
        public string Group();
        public List<RouteMap> RouteMap();
        public void BeforeMount();
        public void OnMount(RouteGroupBuilder builder, RouteMap route) { }
        public void AfterMount() { }
    }
}
