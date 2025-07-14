using SqlSugar;

namespace N_m3u8DL_Web.Entity
{

    public enum OutputStatus
    {
        Downloading = 0,
        Merge=1
    }
    [SugarTable("biz_task_output")]
    public class BizTaskOutPut: BizBase
    {
        public string SaveName { get; set; }

        public OutputStatus OutputStatus { get; set; } = OutputStatus.Downloading;

        public int TaskId { get; set; }

        public string CacheDir { get; set; } 


    }
}
