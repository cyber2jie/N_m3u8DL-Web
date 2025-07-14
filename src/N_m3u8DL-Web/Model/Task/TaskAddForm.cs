using N_m3u8DL_Web.Vars;

namespace N_m3u8DL_Web.Model.Task
{
    public class TaskAddForm
    {
        public string TaskUrl { get; set; }

        public string TaskName { get; set; }

        public string StoreName { get; set; }

        public string? BaseUrl { get; set; } = "";

        public bool? ResetOnDownload { get; set; } = false;

        public string? MergeWay { get; set; } = TaskVars.MergeWay_Binary;

        public string? Headers { get; set; } = "";
    }
}
