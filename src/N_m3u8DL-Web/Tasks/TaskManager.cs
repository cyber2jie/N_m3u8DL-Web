
using N_m3u8DL_Web.Config;
using N_m3u8DL_Web.Entity;
using N_m3u8DL_Web.Util;
using NLog;
using SqlSugar;
using System.Threading.Tasks;

namespace N_m3u8DL_Web.Tasks
{

    public class TaskId<T>
    {
        public TaskId(T id)
        {
            Id = id;
        }

        public T Id { get; set; }
    }
    public class TaskManager
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private static TaskBlockQueue taskQueue;

        private static List<TaskRun> runs = new();

        private static TaskManagerContext context;

        private static DownloadTaskLoader loader;

        private static CancellationTokenSource cancellationTokenSource;
        public static void Init()
        {
            Config.Config config = Context.ContextHolder.Instance.Context.Config;
            SqlSugarClient db= Context.ContextHolder.Instance.Context.GetService<SqlSugarClient>();

            context = new TaskManagerContext()
            {
                Config = config,
                DB = db
            };

            int taskQueueSize = NumberUtil.MaxLimit(config.Download.TaskQueueSize,TaskConstants.MaxTaskQueue);


            Logger.Info($"初始化队列大小:{taskQueueSize}");

            taskQueue = new TaskBlockQueue(taskQueueSize);


            int taskWorkers = NumberUtil.MaxLimit(config.Download.TaskWorkers,TaskConstants.MaxTask);

            Logger.Info($"初始化任务数:{taskWorkers}");

            for (var i=0;i<taskWorkers;i++)
            {
                TaskRun taskRun = new TaskRun(taskQueue)
                {
                    Context=context
                };
                runs.Add(taskRun);

                taskRun.Start();

            }

            cancellationTokenSource=new CancellationTokenSource();

            loader = new DownloadTaskLoader
            {
                Context = context,
                Queue = taskQueue
            };


            int taskLoadTimeout =NumberUtil.MaxLimit(context.Config.Download.TaskLoadTimeout,30);

            Task.Run(async () =>
            {

                await loader.ReloadTaskLoadReset();

                Logger.Info($"Task Load Reset");

                for (;!cancellationTokenSource.Token.IsCancellationRequested;) {

                    try
                    {


                        Logger.Info($"开始加载下载任务");

                        int loaded = await loader.Load();

                        Logger.Info($"共加载{loaded}项任务");

                       await Task.Delay(TimeSpan.FromMinutes(taskLoadTimeout));


                    }catch(Exception e)
                    {
                        Logger.Warn($"加载任务失败,{e.Message}");
                    }
                }
            });





        }

       
        public static async Task ScheduleTask(TaskId<int> id)
        {
            await loader.LoadSingleTaskAsync(id);
        }

        public  static void CleanTaskProcess(TaskId<int> id)
        {


            bool isRunning = false;

            for (var i=0;i<runs.Count;i++)
            {
                TaskRun taskRun = runs[i];
                if (taskRun.HoldTaskId == id.Id)
                {
                    taskRun.Next();
                    isRunning = true;
                }
            }

            if (!isRunning)
            {
                taskQueue.Remove(id);
            }


           BizTaskOutPut outPut = context.DB.Queryable<BizTaskOutPut>().Single(it => it.TaskId == id.Id);
          
            if (outPut != null)
            {
                 
                 FileUtil.DeleteDirectory(context.Config.Storage.Path, outPut.CacheDir);
                 context.DB.Deleteable<BizTaskOutPut>(outPut).ExecuteCommand();
                 context.DB.Deleteable<BizMediaPart>().Where(it => it.TaskId == id.Id).ExecuteCommand();
                 
                 Logger.Info($"清理任务:{id.Id}");
            }



        }

        public static void Dispose()
        {
            runs.ForEach(it=>it.Dispose());
            cancellationTokenSource.Cancel();
            cancellationTokenSource?.Dispose();
            Logger.Info($"TaskManager destory ");
        }
    }
}
