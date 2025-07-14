using N_m3u8DL_Web.Entity;

namespace N_m3u8DL_Web.Tasks
{
    public class DownLoadTask
    {
        public int TaskId { get; set; }

        public string TaskName { get; set; }


        public BizTask BizTask { get; set; }

        public BizTaskOutPut TaskOutPut { get; set; }

        public List<BizMediaPart> MediaParts { get; set; }
        

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}
