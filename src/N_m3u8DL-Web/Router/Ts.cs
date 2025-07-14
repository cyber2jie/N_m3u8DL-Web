
using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Export.Util;
using N_m3u8DL_Web.Util;
using NLog;

namespace N_m3u8DL_Web.Router
{
    public class Ts : AbsRouter
    {
        public override void BeforeMount()
        {
            AddRoute(RouteType.POST, "/download", async (HttpContext context) =>
            {
                var url = context.Request.Form["url"];
                var encryptMethod = context.Request.Form["encryptMethod"];
                var decryptKey = context.Request.Form["decryptKey"];
                var decryptIv = context.Request.Form["decryptIv"];



                logger.Info("download url,{}", url);

                CancellationTokenSource cancellationToken = new();

                byte[] bytes = await DownUtil.DownloadAsync(StringUtil.ToString(url),cancellationToken);

                EncryptMethod method=EncryptMethodHelper.From(StringUtil.ToString(encryptMethod));

                if (EncryptMethod.NONE != method && EncryptMethod.UNKNOWN !=method)
                {
                  bytes= await  DecryptUtil.Decrypt(bytes, method, new DecryptKey() {
                        Key=StringUtil.ToString(decryptKey),
                        IV=StringUtil.ToString(decryptIv)
                    });
                }

                string name = StringUtil.GetUriResource(url);

                if (string.IsNullOrEmpty(name)) name = "download.bin";

                using var stream=new MemoryStream(bytes);

                context.Response.ContentType = "application/octet-stream";
                context.Response.Headers.ContentDisposition = $"attachment; filename=\"{name}\"";
                context.Response.Headers.Server = "N_m3u8DL-Web";

                await stream.CopyToAsync(context.Response.BodyWriter.AsStream());


            });
            
        }

        public override string Group()
        {
            return "/api/ts";
        }
    }
}
