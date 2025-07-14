using System.Collections.Concurrent;

namespace N_m3u8DL_Web.Tasks
{
    public class DownloadThreadPool<T> where T : IDownloadRun
    {
        private readonly List<Thread> _workers;
        private readonly ConcurrentBag<T> _taskQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _stop = false;
        public DownloadThreadPool(int workerCount,List<T> runs)
        {
            _workers = new List<Thread>();
            _taskQueue = new ConcurrentBag<T>();
            _cancellationTokenSource = new CancellationTokenSource();
           
            runs.ForEach(it=>_taskQueue.Add(it));

            for (int i = 0; i < workerCount; i++)
            {
                var worker = new Thread(() =>
                {

                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (_stop) break;

                        if (_taskQueue.TryTake(out var task))
                        {
                            
                            task.Run();

                        }
                        else
                        {
                            break;
                        }

                        
                         

                    }
                });
                worker.Start();
                _workers.Add(worker);
            }
        }

        public void EnqueueTask(T task)
        {
            _taskQueue.Add(task);
        }

        public void Stop()
        {
            _stop = true;
        }
        public void Shutdown()
        {
            _cancellationTokenSource.Cancel();
            foreach (var worker in _workers)
            {
                worker.Join();
            }
        }

        public void WaitAllComplete()
        {
            foreach (var worker in _workers)
            {
                worker.Join();
            }
        }
    }
}
