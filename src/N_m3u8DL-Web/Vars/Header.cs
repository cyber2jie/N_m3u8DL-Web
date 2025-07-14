using N_m3u8DL_Web.Util;

namespace N_m3u8DL_Web.Vars
{
    public class Header
    {
        public string HeaderName { get; set; }
        public string HeaderValue { get; set; }

        public static List<Header> ParseHeaders(string headers)
        {
            return JSONUtil.FromJson<List<Header>>(headers);
        }

        public static Dictionary<string, string> ParseHeaderToDictionary(string headers)
        {
            Dictionary<string, string> headerDic = new();
            try
            {
                List<Header> headersList = ParseHeaders(headers);

                foreach (var header in headersList)
                {
                    headerDic.Add(header.HeaderName, header.HeaderValue);
                }
            }catch(Exception e){}

            return headerDic;
        }
    }
}
