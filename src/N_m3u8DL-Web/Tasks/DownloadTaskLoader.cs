using N_m3u8DL_RE.Common.Entity;
using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Export.Entity;
using N_m3u8DL_RE.Export.Util;
using N_m3u8DL_Web.Entity;
using N_m3u8DL_Web.Vars;
using NLog;
using System.ComponentModel;

namespace N_m3u8DL_Web.Tasks
{
    public class DownloadTaskLoader
    {

        private Logger logger = LogManager.GetCurrentClassLogger();

        public TaskBlockQueue Queue { get; set; }

        public  TaskManagerContext Context { get; set; }


        public  async Task ReloadTaskLoadReset()
        {

           await  Context.DB.Updateable<BizTask>().SetColumns("TaskLoad", false).Where(it => it.TaskLoad != false ).ExecuteCommandAsync();

        }

        public async Task<int> Load()
        {
            try
            {

                List<BizTask> tasks = Context.DB.Queryable<BizTask>().Where(it => it.TaskStatus != TaskVars.TaskStatus_Finished)
                     .Where(it => it.TaskLoad != true).ToList();

                foreach (var task in tasks)
                {
                    task.TaskLoad = true;

                    await LoadTask(task);
                }


                Context.DB.Updateable<BizTask>(tasks).UpdateColumns(
                    it => new { it.TaskLoad }
                    ).ExecuteCommand();

                return tasks.Count;
            }catch(Exception e)
            {
                logger.Error("Loading Task Occur Error,{}", e.Message);
                return 0;
            }
        }

        public async Task LoadSingleTaskAsync(TaskId<int> taskId)
        {
            logger.Info("LoadSingleTaskAsync, taskId: {}", taskId.Id);

            BizTask bizTask = Context.DB.Queryable<BizTask>().Where(it => it.Id == taskId.Id).Single();

           

            if (bizTask != null)
            {
                await LoadTask(bizTask);

                bizTask.TaskLoad = true;

                Context.DB.Updateable<BizTask>(bizTask)
                    .UpdateColumns(it => new { it.TaskLoad })
                    .ExecuteCommand();

            }
            else
            {
                logger.Info($"task miss:{taskId}");
            }

        }


        private BizTaskOutPut getOrCreate(BizTask task)
        {
            BizTaskOutPut taskOutPut = Context.DB.Queryable<BizTaskOutPut>().Single(it => it.TaskId == task.Id);

            try
            {

                int extractCount= Context.DB.Queryable<BizMediaPart>()
                            .Where(it => it.TaskId == task.Id).Count();



                if (taskOutPut == null || task.ResetOnDownload || extractCount<=0 )
                {

                    M3u8Info info = ExtractUtil.ExtractFromUrl(task.TaskUrl).GetAwaiter().GetResult();

                    if (info != null && info.VideoStreams.Count > 0)
                    {

                        Playlist playlist = info.VideoStreams[0].Playlist;


                        if (playlist != null && playlist.MediaParts.Count > 0)
                        {


                            if (taskOutPut == null)
                            {

                                BizTaskOutPut _out = new BizTaskOutPut();

                                _out.TaskId = task.Id;
                                _out.SaveName = task.StoreName;
                                _out.CacheDir = Guid.NewGuid().ToString();
                                _out.OutputStatus = OutputStatus.Downloading;

                                int _id = Context.DB.Insertable<BizTaskOutPut>(_out).ExecuteReturnIdentity();

                                _out.Id = _id;

                                taskOutPut = _out;
                            }

                            List<MediaSegment> segments = playlist.MediaParts[0].MediaSegments;

                            List<BizMediaPart> mediaParts = Context.DB.Queryable<BizMediaPart>()
                            .Where(it => it.TaskId == task.Id).ToList();

                            if (mediaParts.Count > 0)
                            {
                                foreach (var seg in segments)
                                {

                                    var mediaPart = mediaParts.Where(it => it.Index == seg.Index).FirstOrDefault();

                                    if (mediaPart != null)
                                    {
                                        mediaPart.Url = seg.Url;
                                        if (seg.EncryptInfo != null)
                                        {
                                            mediaPart.EncryptKey = seg.EncryptInfo.KeyHex;
                                            mediaPart.EncryptIV = seg.EncryptInfo.IVHex;
                                        }

                                    }

                                }

                                Context.DB.Updateable<BizMediaPart>(mediaParts)
                                    .UpdateColumns(it => new { it.Url, it.EncryptIV, it.EncryptKey }).ExecuteCommand();

                            }
                            else
                            {

                                List<BizMediaPart> mps = new List<BizMediaPart>();
                                foreach (var seg in segments)
                                {
                                    BizMediaPart mp = new BizMediaPart();

                                    mp.TaskId = task.Id;

                                    mp.Url = seg.Url;
                                    mp.Duration = seg.Duration;

                                    mp.Index = (int)seg.Index;

                                    mp.Download = false;

                                    if (seg.EncryptInfo != null)
                                    {
                                        mp.EncryptMethod = EncryptMethodHelper.GetEncryptMethodString(seg.EncryptInfo.Method);

                                        mp.EncryptIV = seg.EncryptInfo.IVHex;
                                        mp.EncryptKey = seg.EncryptInfo.KeyHex;

                                        mp.IsEncrypted = seg.IsEncrypted;
                                    }

                                    mps.Add(mp);
                                }


                                Context.DB.Insertable<BizMediaPart>(mps).ExecuteCommand();




                            }





                        }


                    }
                }


            }catch(Exception e)
            {
                logger.Error("Make Output Error ,{}", e.Message);
            }

            return taskOutPut;
        }


        public async Task LoadTask(BizTask bizTask)
        {

            logger.Info("LoadTask, taskId: {},taskName: {}", bizTask.Id,bizTask.TaskName);

            await Task.Run(() =>
            {



               BizTaskOutPut outPut = getOrCreate(bizTask);

               if (outPut !=null )
                {

                    DownLoadTask task = new DownLoadTask();

                    task.TaskName = bizTask.TaskName;
                    task.TaskId = bizTask.Id;
                    task.BizTask = bizTask;
                    task.TaskOutPut = outPut;

                    List<BizMediaPart> mediaParts = Context.DB.Queryable<BizMediaPart>()
                    .Where(it => it.TaskId == bizTask.Id).ToList();

                    task.MediaParts = mediaParts;

                    task.Headers = Header.ParseHeaderToDictionary(bizTask.Headers);

                    Queue.Add(task);


                }
                else
                {

                    logger.Warn("taskName: {},outPut is null", bizTask.Id,bizTask.TaskName);


                }





            });


        }
    }
}
