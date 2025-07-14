using SqlSugar;

namespace N_m3u8DL_Web.Service.Impl
{
    public class BaseDbService
    {
        protected SqlSugarClient GetDB()
        {
            var db = Context.ContextHolder.Instance.Context.GetService<SqlSugarClient>();

            return db;
        }
    }
}
