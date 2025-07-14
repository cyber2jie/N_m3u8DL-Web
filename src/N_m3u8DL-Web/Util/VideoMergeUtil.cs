using N_m3u8DL_RE.Common.Entity;
using N_m3u8DL_RE.Common.Resource;
using N_m3u8DL_RE.Common.Util;
using N_m3u8DL_RE.Util;

namespace N_m3u8DL_Web.Util
{
    public class VideoMergeUtil
    {
        public static void Merge(List<string> files,string outputName)
        {
            MergeUtil.CombineMultipleFilesIntoSingleFile(files.ToArray(), outputName);
        }

        public static void MergeByFFmpeg(List<string> files, string outputName)
        {
            string[] filesArray= files.ToArray();

            if (filesArray.Length >= 1800)
            {
                filesArray = MergeUtil.PartialCombineMultipleFiles(filesArray);
               
            }

            MergeUtil.MergeByFFmpeg(GlobalUtil.FindExecutable("ffmpeg"), filesArray, Path.ChangeExtension(outputName, null),"mp4", false);

        }

    }
}
