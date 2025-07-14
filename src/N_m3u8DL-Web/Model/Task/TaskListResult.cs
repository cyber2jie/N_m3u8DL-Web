using N_m3u8DL_Web.Entity;

namespace N_m3u8DL_Web.Model.Task
{
    public class TaskListResult: Result
    {

        public int TotalCount { get; set; }


        public int Count { get; set; }

        public List<BizTask> Tasks { get; set; }


    }
}
