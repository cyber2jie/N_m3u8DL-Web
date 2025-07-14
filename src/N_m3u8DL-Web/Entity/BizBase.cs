using SqlSugar;

namespace N_m3u8DL_Web.Entity
{
    public class BizBase
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public DateTime CreateAt{ get; set; } = DateTime.Now;
    }
}
