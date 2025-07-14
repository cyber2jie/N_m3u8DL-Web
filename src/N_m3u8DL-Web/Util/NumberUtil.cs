namespace N_m3u8DL_Web.Util
{
    public class NumberUtil
    {
        public static int MaxLimit(int a, int max)
        {
            if(a <=0 ) return max;
            return a > max ? max : a;
        }
    }
}
