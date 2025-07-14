using NLog;

namespace N_m3u8DL_Web.Util
{
    public class FileUtil
    {
        private static Logger logger=LogManager.GetCurrentClassLogger();


        public static bool WriteFile(string basePath, string name, byte[] content)
        {
            try
            {
                MakeDirectory(basePath);
                File.WriteAllBytes(Path.Combine(basePath, name), content);
                return true;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public static bool MakeDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }
        }
        public static bool DeleteDirectory(string baseDir,string path)
        {
            try
            {
                string fullPath= Path.Combine(baseDir, path);

                if (!Directory.Exists(fullPath))
                {
                    return false;
                }


                Directory.Delete(fullPath, true);

            }
            catch (Exception e)
            {
                logger.Warn($"delete dir {path} error,{e.Message}");
                return false;
            }
            return true;

        }
    }
}
