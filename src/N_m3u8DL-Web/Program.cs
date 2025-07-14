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

            // ��ʼ��������
            Context.ContextHolder.From(config);

            // DB
            DB.Index.Init(Context.ContextHolder.Instance.Context);


            // Entity,������
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

            // ��ӷ���
            app.MapControllers();


            //ע���м��
            Middleware.Index.Use(app);

            //ע��·��
            Router.Index.Register(app);


           

            //ʹ��swagger
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "N_m3u8DL-Web");
            });

            //�˳�����
            new Exit.Exit(() => {


                DB.Index.Destroy();

                TaskManager.Dispose();

                NLog.LogManager.Shutdown();

                

            }).RegisterExitHandler();

            app.Run(config.Web.RunUrl);



        }
    }


}
