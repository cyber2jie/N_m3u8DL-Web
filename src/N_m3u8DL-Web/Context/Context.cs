
namespace N_m3u8DL_Web.Context
{


    public class Context
    {

        public  Config.Config Config {  get; set; }

        private  Dictionary<string,object?> Services { get; set; }= new Dictionary<string,object?>();

        public Context(Config.Config config)
        {
            Config = config;
        }
        public void AddService<T>(T service)
        {
            Services.Add(typeof(T).Name, service);
        }
        public T GetService<T>()
        {
            return (T)Services[typeof(T).Name];
        }
    }
    public class ContextHolder
    {

        public  readonly Context Context;

        public ContextHolder(Config.Config config)
        {
            Context = new Context(config);
        }

        public static ContextHolder Instance;
        public static void From(Config.Config config)
        {
            Instance = new ContextHolder(config);
        }

    }
}
