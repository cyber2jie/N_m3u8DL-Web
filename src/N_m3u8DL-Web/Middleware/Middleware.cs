using N_m3u8DL_Web.Context;

using JWT.Builder;
using JWT.Algorithms;
using NLog;
using Spectre.Console;
using Dm.util;
using Newtonsoft.Json.Linq;
using N_m3u8DL_Web.Service.Impl;
using N_m3u8DL_Web.Util;
using N_m3u8DL_Web.Model;

namespace N_m3u8DL_Web.Middleware
{

    public class Middleware
    {
    
        private static Logger logger=NLog.LogManager.GetCurrentClassLogger();

        public static Func<HttpContext, Func<Task>, Task> RequestLog= async (HttpContext ctx,Func<Task> next) =>
        {

            var now = DateTime.Now;
            
            await next();
            var text = $"{ctx.Request.Method} {ctx.Request.Path}  status [yellow bold]{ctx.Response.StatusCode} [/]  time cost {"[green]" + (DateTime.Now - now).TotalMilliseconds + "[/]"}s";
            
            AnsiConsole.MarkupLine(text);


        };

        
        public static Func<HttpContext, Func<Task>, Task> AuthCheck = async (HttpContext ctx,Func<Task> next) =>
        {
            if(!ctx.Request.Path.StartsWithSegments("/api")
            ||
            ctx.Request.Path.StartsWithSegments("/api/login"))
            {
                await next();
                return;

            }

            Config.Config config = ContextHolder.Instance.Context.Config;

            if (!config.Admin.Auth)
            {
                await next();
                return;
            }

            if (!ctx.Request.Headers.ContainsKey("Authorization"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsync(JSONUtil.ToJson(CommonResult.Fail("Authorization header is missing.")));
                return;
            }

            string authStart="Bearer ";

            string authHeader = ctx.Request.Headers["Authorization"];

            if (!authHeader.StartsWith(authStart, StringComparison.OrdinalIgnoreCase))
            {
                await ctx.Response.WriteAsync(JSONUtil.ToJson(CommonResult.Fail("Invalid token format.")));
                return;
            }

          
            var token= authHeader.Substring(authStart.Length).Trim();

            var claims = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(config.Admin.Certificate)
            .MustVerifySignature()
            .Decode(token);

            JObject jobject = JObject.Parse(claims);

            string loginId=jobject["loginId"].ToString();

            if (!ValidateCache.Instance.ContainsKey(loginId))
            {
                await ctx.Response.WriteAsync(JSONUtil.ToJson(CommonResult.Fail("loginId Error.")));
                return;
            }

            UserInfo userInfo= ValidateCache.Instance[loginId];

          

            long now=DateTimeOffset.Now.ToUnixTimeSeconds();

            if (userInfo.ExpireTime<now)
            {
                await ctx.Response.WriteAsync(JSONUtil.ToJson(CommonResult.Fail("Token Expire.")));
                return;

            }

            /*
            if (!userInfo.User.Equals(jobject["user"].ToString()))
            {
                await ctx.Response.WriteAsync("User Not Match Error.");
                return;
            }
            */

            await next();
        };
    }
  
}
