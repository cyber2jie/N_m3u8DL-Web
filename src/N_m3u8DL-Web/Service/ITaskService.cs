using N_m3u8DL_Web.Model;
using N_m3u8DL_Web.Model.Task;

namespace N_m3u8DL_Web.Service
{
    public interface ITaskService
    {
        public TaskAddResult Add(TaskAddForm taskAdd);

        public CommonResult Delete(int taskId);

        public TaskListResult List(TaskListForm taskList);

        public TaskStatusResult Status(TaskStatusForm taskStatus);
    }
}
