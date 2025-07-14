using N_m3u8DL_RE.Common.Entity;

namespace N_m3u8DL_RE.Export.Entity
{
    public  class M3u8Info
    {

        public List<StreamSpec> AudioStreams { get; set; }
        public List<StreamSpec> VideoStreams { get; set; }
        public List<StreamSpec> SubtitleStreams { get; set; }



    }
}
