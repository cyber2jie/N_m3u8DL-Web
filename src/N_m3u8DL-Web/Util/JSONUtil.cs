using Newtonsoft.Json;

namespace N_m3u8DL_Web.Util
{
    public class JSONUtil
    {
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
