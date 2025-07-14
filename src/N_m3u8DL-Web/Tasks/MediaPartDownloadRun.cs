using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Export.Util;
using N_m3u8DL_Web.Entity;
using N_m3u8DL_Web.Util;
using NLog;
using SqlSugar;

namespace N_m3u8DL_Web.Tasks
{
    public class MediaPartDownloadRun : IDownloadRun
    {

        private Logger logger = LogManager.GetCurrentClassLogger();
        public TaskManagerContext Context { get; set; }

        public SqlSugarClient DB { get; set; }
        public DownLoadTask DownLoadTask { get; set; }

        public BizMediaPart MediaPart { get; set; }


        private String makeUrl()
        {
            if (!string.IsNullOrEmpty(DownLoadTask.BizTask.BaseUrl))
            {
                return DownLoadTask.BizTask.BaseUrl + MediaPart.Url;

            }

            return MediaPart.Url;
        }

        private String getBasePath() { 
        
            return Path.Combine(Context.Config.Storage.Path, DownLoadTask.TaskOutPut.CacheDir);
        }

        private byte[] descrypt(byte[] content)
        {
            EncryptMethod method = EncryptMethodHelper.GetEncryptMethod(MediaPart.EncryptMethod);

            if (EncryptMethod.NONE != method && EncryptMethod.UNKNOWN != method)
            {
                content = DecryptUtil.Decrypt(content, method, new DecryptKey()
                {
                    Key = StringUtil.ToString(MediaPart.EncryptKey),
                    IV = StringUtil.ToString(MediaPart.EncryptIV)
                }).GetAwaiter().GetResult();
            }
            return content;
        }
        public void Run()
        {

            if ( MediaPart !=null )
            {
                logger.Info($"MediaPartDownloadRun[{DownLoadTask.BizTask.TaskName}] [{MediaPart.Index}] Start");


                byte[] content= DownUtil.DownloadAsync(makeUrl(), new CancellationTokenSource(),headers:DownLoadTask.Headers).GetAwaiter().GetResult();

                FileUtil.WriteFile(getBasePath(), $"{MediaPart.Index}.ts",descrypt(content));

                MediaPart.Download = true;
                lock (DB)
                {
                    DB.Updateable<BizMediaPart>().
                         SetColumns(it => it.Download == true)
                        .Where(it => it.Id == MediaPart.Id)
                        .ExecuteCommand();

                }

                logger.Info($"MediaPartDownloadRun[{DownLoadTask.BizTask.TaskName}][{MediaPart.Id}] Complete");
            }
            

        }
    }
}
