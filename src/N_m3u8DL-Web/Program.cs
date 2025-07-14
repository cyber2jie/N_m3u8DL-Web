using N_m3u8DL_Web.Config;

using N_m3u8DL_Web.Middleware;
using N_m3u8DL_Web.Router;
using N_m3u8DL_Web.Exit;


using NLog;
using NLog.Web;
using N_m3u8DL_Web.Tasks;


using Microsoft.OpenApi.Models;

namespace N_m3u8DL_Web
{
    
    internal class Program
    {
        public static void Main(string[] args)
        {


            NLog.LogManager.Setup().LoadConfigurationFromAppSettings();

            Config.Config config=Config.Config.Parse("web.toml");

            // 初始化上下文
            Context.ContextHolder.From(config);

            // DB
            DB.Index.Init(Context.ContextHolder.Instance.Context);


            // Entity,建立表
            Entity.Index.Init();


            TaskManager.Init();

            var builder = WebApplication.CreateBuilder(args);

            builder.Environment.ApplicationName = config.Web.Name;

            builder.Environment.WebRootPath = config.Web.RootPath;

            
            builder.Logging.ClearProviders();

            builder.Host.UseNLog();
            

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "N_m3u8DL-Web", Version = "v1" });
            });
            
            Service.Index.Scoped(builder.Services);


            var app = builder.Build();

            // 添加服务
            app.MapControllers();


            //注册中间件
            Middleware.Index.Use(app);

            //注册路由
            Router.Index.Register(app);


           

            //使用swagger
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "N_m3u8DL-Web");
            });

            //退出处理
            new Exit.Exit(() => {


                DB.Index.Destroy();

                TaskManager.Dispose();

                NLog.LogManager.Shutdown();

                

            }).RegisterExitHandler();

            app.Run(config.Web.RunUrl);



        }
    }


}
