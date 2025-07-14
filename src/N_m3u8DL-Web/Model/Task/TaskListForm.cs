namespace N_m3u8DL_Web.Model.Task
{
    public class TaskListForm
    {
        public string? TaskName { get; set; }

        public DateTime? CreateAtMin { get; set; }

        public DateTime? CreateAtMax { get; set; }


        public Pagination? Pagination { get; set; } = new Pagination();


    }
}
