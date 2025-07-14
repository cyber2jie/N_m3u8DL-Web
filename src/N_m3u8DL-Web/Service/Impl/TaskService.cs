using N_m3u8DL_Web.Entity;
using N_m3u8DL_Web.Model.Task;
using N_m3u8DL_Web.Util;

using N_m3u8DL_Web.Model;
using SqlSugar;
using System.Linq.Expressions;
using System.Threading.Tasks;
using N_m3u8DL_Web.Tasks;
using N_m3u8DL_Web.Vars;
using Dm.util;

namespace N_m3u8DL_Web.Service.Impl
{
    public class TaskService : BaseDbService, ITaskService
    {
        public TaskAddResult Add(TaskAddForm taskAdd)
        {

            return SafeRunUtil.Run<TaskAddResult>(() =>
            {
                BizTask task = BeanUtil.ToBean<TaskAddForm, BizTask>(taskAdd);


                FormRequireUtil.Require(task, "TaskUrl", "TaskName", "StoreName");


                task.TaskStatus = TaskVars.TaskStatus_Running;

                int taskId = GetDB().Insertable(task).ExecuteReturnIdentity();


                TaskAddResult result = new();

                result.TaskId = taskId.ToString();
                result.TaskName = task.TaskName;
                result.Message = Constants.ResultSuccess;


                Task.Run(async () =>
                {
                    TaskManager.ScheduleTask(new TaskId<int>(taskId)).GetAwaiter().GetResult();
                });

                return result;
            }).Error((ex) =>
            {
                return new TaskAddResult
                {
                    Message = Constants.ResultFail,
                    Info = ex.Message
                };
            }).Get();

        }

        public CommonResult Delete(int taskId)
        {

            if (taskId == null)
            {

                throw new N_m3u8DL_Web.Exceptions.N_m3u8DLWebException(@" ""taskId"" is required");
            }

            GetDB().Deleteable<BizTask>(new BizTask() { Id = taskId }).ExecuteCommand();

            Task.Run(() =>
            {
                TaskManager.CleanTaskProcess(new TaskId<int>(taskId));
            });

            return CommonResult.Success(null);
        }

        public TaskListResult List(TaskListForm taskList)
        {
            TaskListResult result = new();

            int pageNum = 1, pageSize = 10, total = 0;


            if (taskList.Pagination != null)
            {
                if (taskList.Pagination.PageNum != null && taskList.Pagination.PageNum > 0)
                {
                    pageNum = taskList.Pagination.PageNum.Value;
                }

                if (taskList.Pagination.PageSize != null && taskList.Pagination.PageSize > 0)
                {
                    pageSize = taskList.Pagination.PageSize.Value;
                }

            }


            Expression<Func<BizTask, bool>> exp = Expressionable.Create<BizTask>()
                 .AndIF(!string.IsNullOrEmpty(taskList.TaskName), x => x.TaskName.Contains(taskList.TaskName))
                 .AndIF(taskList.CreateAtMin != null, x => x.CreateAt >= taskList.CreateAtMin)
                 .AndIF(taskList.CreateAtMax != null, x => x.CreateAt <= taskList.CreateAtMax)
                 .ToExpression();



            List<BizTask> tasks=GetDB().Queryable<BizTask>().Where(exp).ToPageList(pageNum, pageSize, ref total);


            result.Tasks = tasks;
            result.Count = tasks.Count();
            result.Message = Constants.ResultSuccess;
            result.TotalCount = total;

            return result;
        }

        public TaskStatusResult Status(TaskStatusForm taskStatus)
        {
            int taskId = taskStatus.TaskId;

            BizTask bizTask= GetDB().Queryable<BizTask>().Where(it => it.Id == taskId).Single();

            TaskStatusResult result = new TaskStatusResult();

            if (bizTask !=null)
            {

               result= BeanUtil.ToBean<BizTask, TaskStatusResult>(bizTask);

               result.TaskId = taskId;

               List<BizMediaPart> mediaParts = GetDB().Queryable<BizMediaPart>().Where(it => it.TaskId == taskId).ToList();

               List<TaskMediaPartStatus> mediaPartStatuses = new();


                int total = mediaParts.Count;
                int download = 0;

                foreach (var m in mediaParts)
                {
                    TaskMediaPartStatus status= BeanUtil.ToBean<BizMediaPart, TaskMediaPartStatus>(m);

                    if (m.Download)
                    {
                        download++;
                    }
                    mediaPartStatuses.Add(status);
                }

                string progress = (((double)download / (double)total)).ToString("0.00%");


                result.TotalSegments = total;
                result.DownloadSegments = download;

                result.Progress = progress;

                result.TaskMediaParts = mediaPartStatuses;





            }



            return result;
        }
    }
}
