using N_m3u8DL_Web.Entity;
using N_m3u8DL_Web.Util;
using N_m3u8DL_Web.Vars;
using NLog;
using SqlSugar;
using System.Threading.Tasks;

namespace N_m3u8DL_Web.Tasks
{
    public class TaskRun: IDisposable
    {

        private Logger logger = LogManager.GetCurrentClassLogger();

        private TaskBlockQueue queue;
        public CancellationTokenSource cancellationTokenSource { get; }

        public string TaskId { get; }

        public TaskManagerContext Context { get; set; }

        public int HoldTaskId { get; set; }

        private SqlSugarClient db { get; set; }

        public DownloadThreadPool<MediaPartDownloadRun> Pool { get; set; }

        public TaskRun(TaskBlockQueue queue)
        {
            this.queue = queue;
            cancellationTokenSource = new CancellationTokenSource();
            TaskId = Guid.NewGuid().ToString();
            db=DB.Index.GetClient();
        }

        public void Start()
        {
            logger.Info($"TaskRun[{TaskId}] Start");

            int downThread=NumberUtil.MaxLimit(Context.Config.Download.TaskDownloadThread,TaskConstants.MaxTaskDownloadThread);

            Task.Run(() =>
                {
                    for (;!cancellationTokenSource.Token.IsCancellationRequested;)
                    {
                       
                        var task = queue.Take();

                        logger.Info($"TaskRun[{TaskId}] take task[{task.TaskId}][{task.TaskName}]");

                        HoldTaskId=task.TaskId;

                        Pool = new DownloadThreadPool<MediaPartDownloadRun>(downThread,GetMediaPartDownloadRun(task));


                        Pool.WaitAllComplete();


                        Merge(task);

                        logger.Info($"TaskRun[{TaskId}] task[{task.TaskId}][{task.TaskName}] complete");
                        
                    }

                },cancellationTokenSource.Token);

            

        }

        public void Next()
        {
            if (Pool != null)
            {
                Pool.Stop();
                Pool.Shutdown();
            }

        }
        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource?.Dispose();
            Next();
            logger.Info($"TaskRun[{TaskId}] destory ");
        }

        private void Merge(DownLoadTask task)
        {

           string baseDir= Path.Combine(Context.Config.Storage.Path, task.TaskOutPut.CacheDir);

            List<string> files = new();

            foreach (var mediaPart in task.MediaParts.OrderBy(x => x.Index))
            {
                files.Add(Path.Combine(baseDir, $"{mediaPart.Index}.ts"));
            }

            string outPutFileName= Path.Combine(Context.Config.Storage.Path, $"{task.BizTask.TaskName}.mp4");


            if (StringUtil.TrimEqual(task.BizTask.MergeWay, TaskVars.MergeWay_Binary))
            {
                
                VideoMergeUtil.Merge(files, outPutFileName);

            }
            else
            {
                VideoMergeUtil.MergeByFFmpeg(files, outPutFileName);
            }

            Context.DB.Updateable<BizTask>()
                .SetColumns(it=>it.TaskStatus == TaskVars.TaskStatus_Finished)
                .Where(it=>it.Id == task.BizTask.Id)
                .ExecuteCommand();

        }

        private List<MediaPartDownloadRun> GetMediaPartDownloadRun(DownLoadTask task)
        {
            var runs = new List<MediaPartDownloadRun>();

            foreach (var part in task.MediaParts)
            {
                if (part.Download) continue;

                var run = new MediaPartDownloadRun()
                {
                    DownLoadTask = task,
                    MediaPart = part,
                    Context = Context,
                    DB = db
                };


                runs.Add(run);
            }

            return runs;
        }
    }
}
