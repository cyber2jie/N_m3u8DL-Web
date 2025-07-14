using N_m3u8DL_Web.Context;
using NLog;

using SqlSugar;
using System.Collections.Concurrent;

namespace N_m3u8DL_Web.DB
{

    internal class Constants
    {
        public static string DB_PATH = "N_m3u8.db";
        public static string ConnectionString = $"DataSource={DB_PATH}";

    }
    public class Index
    {

        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static SqlSugarClient db;


        private static ConcurrentDictionary<string, SqlSugarClient> clients = new ConcurrentDictionary<string, SqlSugarClient>();


        public static void Init(Context.Context context)
        {

            logger.Info("Init DB ... ");
            db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Sqlite,
                ConnectionString = Constants.ConnectionString,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    SqliteCodeFirstEnableDefaultValue = true
                }
            });



            db.Open();

            db.DbMaintenance.CreateDatabase();

            logger.Info("Done");

            context.AddService<SqlSugarClient>(db);


        }

        public static void Destroy()
        {
            logger.Info("Destroy DB...");
            db?.Dispose();

            foreach(var client in clients.Values)
            {
                client.Dispose();
            }

            logger.Info("Done");
        }

        public static SqlSugarClient GetClient()
        {
            //按线程获取
            string clientKey = $"client-{Thread.CurrentThread.ManagedThreadId}";

            if (!clients.ContainsKey(clientKey))
            {
                SqlSugarClient client = new SqlSugarClient(new ConnectionConfig()
                {
                    DbType = DbType.Sqlite,
                    ConnectionString = Constants.ConnectionString,
                    IsAutoCloseConnection = true,
                    MoreSettings = new ConnMoreSettings()
                    {
                        SqliteCodeFirstEnableDefaultValue = true
                    }
                });
                client.Open();
                clients[clientKey] = client;


            }
            return clients[clientKey];
        }




    }
}
