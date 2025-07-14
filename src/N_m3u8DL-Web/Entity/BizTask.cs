using N_m3u8DL_Web.Vars;
using SqlSugar;
namespace N_m3u8DL_Web.Entity
{

    [SugarTable("biz_task")]
    public class BizTask: BizBase
    {

        public string TaskUrl { get; set; }

        public string TaskName { get; set; }

        public string StoreName { get; set; }

        public string? BaseUrl { get; set; } = "";

        public string? MergeWay { get; set; } = TaskVars.MergeWay_Binary;

        public bool ResetOnDownload { get; set; } = false;


        public string? TaskStatus { get; set; } = "";


        public bool TaskLoad { get; set; } = false;


        public string? Headers { get; set; } = "";

    }
}
