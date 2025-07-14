namespace N_m3u8DL_Web.Util
{
    public class StringUtil
    {
        public static string ToString(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        public static string GetUriResource(string name)
        {
            var n= name == null ? "" : name.Split("/").Last();
            var idx=n.IndexOf("?");
            if (idx > 0)
            {
                return n.Substring(0, idx);
            }
            return n;
        }

        public static bool TrimEqual(string a, string b)
        {
            return a?.Trim() == b?.Trim();
        }
    }
}
