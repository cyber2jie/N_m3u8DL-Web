using N_m3u8DL_RE.CommandLine;
using N_m3u8DL_RE.Common.Entity;
using N_m3u8DL_RE.Parser;
using N_m3u8DL_RE.Parser.Config;


namespace N_m3u8DL_RE.Export.Entity
{
    public  class M3u8DownloadInfo
    {
        public MyOption Option { get; set; }

        public ParserConfig ParserConfig { get; set; }

        public StreamExtractor StreamExtractor { get; set; }

        public  List<StreamSpec> SelectedStreams { get; set; }

        public bool LivingFlag { get; set; }

    }
}
