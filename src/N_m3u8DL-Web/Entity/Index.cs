using SqlSugar;
namespace N_m3u8DL_Web.Entity
{
    public class Index
    {
        public static void Init()
        {
            var db = Context.ContextHolder.Instance.Context.GetService<SqlSugarClient>();

            db.CodeFirst.SetStringDefaultLength(256)
                .InitTables(
                typeof(BizTask),
                typeof(BizTaskOutPut),
                typeof(BizMediaPart)
                )
                ;
        }
    }
}
