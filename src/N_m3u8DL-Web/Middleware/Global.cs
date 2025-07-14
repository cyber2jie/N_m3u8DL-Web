using N_m3u8DL_Web.Model;
using N_m3u8DL_Web.Util;
using NLog;
using System.Net;

namespace N_m3u8DL_Web.Middleware
{
    public class GlobalMiddleware
    {
        private Logger logger=NLog.LogManager.GetCurrentClassLogger();
        private RequestDelegate _next;

        public GlobalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            try
            {
                await _next(context);
            }catch(Exception e)
            {
                logger.Error(e.StackTrace);
                context.Response.StatusCode =(int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JSONUtil.ToJson(CommonResult.Fail(e.Message)));
                return;
            }

            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                await context.Response.WriteAsync(JSONUtil.ToJson(CommonResult.Fail("404 not found")));
            }

           
        }
    }
}
