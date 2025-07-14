using N_m3u8DL_Web.Service.Impl;

namespace N_m3u8DL_Web.Service
{
    public class Index
    {
        public static void Scoped(IServiceCollection service)
        {
            service.AddScoped<ILoginService, LoginService>();

            service.AddScoped<ITaskService, TaskService>();



        }
    }
}
