using N_m3u8DL_Web.Config;
using SqlSugar;

namespace N_m3u8DL_Web.Tasks
{
    public class TaskManagerContext
    {
        public Config.Config Config { get; set; }

        public SqlSugarClient DB { get; set; }

    }
}
