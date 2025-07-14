namespace N_m3u8DL_Web.Middleware
{
    public class Index
    {
        public static void Use(WebApplication app)
        {

            app.UseStaticFiles();

            app.UseFileServer();

            app.Use(Middleware.RequestLog);
            app.Use(Middleware.AuthCheck);
            app.UseMiddleware<GlobalMiddleware>();
        }
    }
}
