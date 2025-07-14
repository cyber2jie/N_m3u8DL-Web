using NLog;
using System.Collections.Concurrent;

namespace N_m3u8DL_Web.Tasks
{
    public class TaskBlockQueue
    {
        private Logger logger=LogManager.GetCurrentClassLogger();

        private BlockingCollection<DownLoadTask> queue;

        private List<TaskId<int>> remove;
        private SemaphoreSlim semaphore;

        public TaskBlockQueue(int size)
        {
            queue = new BlockingCollection<DownLoadTask>(new ConcurrentQueue<DownLoadTask>(),size);
            remove = new();
            semaphore = new SemaphoreSlim(1,1);
        }

        public void Add(DownLoadTask task)
        {
            queue.Add(task);
        }

        public void Remove(TaskId<int> taskId)
        {
            remove.Add(taskId);
        }
        public DownLoadTask Take()
        {
            for (;;)
            {
                DownLoadTask task = queue.Take();

                bool isRemove = false;
                try
                {
                    semaphore.Wait();
                    if (remove.Where(x => x.Id == task.TaskId).Count() > 0)
                    {

                        remove.RemoveAll(x => x.Id == task.TaskId);
                        isRemove = true;

                        logger.Warn($"TaskId {task.TaskId} is removed,skip download");

                    }
                }
                finally
                {
                    semaphore.Release();
                }
                if(!isRemove) return task;

            }

        }
    }
}
