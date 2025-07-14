using SqlSugar;

namespace N_m3u8DL_Web.Entity
{

    [SugarTable("biz_media_part")]
    public class BizMediaPart: BizBase
    {

        public int Index { get; set;}


        public double Duration { get; set;}



        public bool IsEncrypted { get; set; } = false;


        public string EncryptKey { get; set; } = "";


        public string EncryptIV { get; set; } = "";

        public string Url { get; set; }

        public int TaskId { get; set; }

        public bool Download { get; set; } = false;

        public string EncryptMethod { get; set; } = "";


    }
}
