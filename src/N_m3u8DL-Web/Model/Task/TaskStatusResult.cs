namespace N_m3u8DL_Web.Model.Task
{

    public class TaskMediaPartStatus
    {
        public int Index { get; set; }


        public double Duration { get; set; }

        public bool IsEncrypted { get; set; } = false;
        public string EncryptKey { get; set; } = "";

        public string EncryptIV { get; set; } = "";

        public string Url { get; set; }

        public int TaskId { get; set; }

        public bool Download { get; set; } = false;

        public string EncryptMethod { get; set; } = "";

    }
    public class TaskStatusResult
    {
        public int TaskId { get; set; }

        public string TaskUrl { get; set; }

        public string TaskName { get; set; }

        public string StoreName { get; set; }

        public bool ResetOnDownload { get; set; } = false;
        public string? TaskStatus { get; set; } = "";

        public bool TaskLoad { get; set; } = false;

        public List<TaskMediaPartStatus> TaskMediaParts { get; set; } = new();

        public int TotalSegments { get; set; } = 0;

        public int DownloadSegments { get; set; } = 0;

        public string Progress { get; set; } = "";
    }
}
