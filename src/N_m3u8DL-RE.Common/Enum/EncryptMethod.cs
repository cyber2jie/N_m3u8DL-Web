namespace N_m3u8DL_RE.Common.Enum;

public enum EncryptMethod
{
    NONE,
    AES_128,
    AES_128_ECB,
    SAMPLE_AES,
    SAMPLE_AES_CTR,
    CENC,
    CHACHA20,
    UNKNOWN
}

public class EncryptMethodHelper
{ 

    public static EncryptMethod From(string candidate)
    {
        try
        {
            return GetEncryptMethod(int.Parse(candidate));
        }catch( Exception e)
        {
            return GetEncryptMethod(candidate);
        }
    }
    public static EncryptMethod GetEncryptMethod(string method)
    { 
        switch (method)
        {
            case "AES-128":
                return EncryptMethod.AES_128;
            case "AES-128-ECB":
                return EncryptMethod.AES_128_ECB;
            case "SAMPLE-AES":
                return EncryptMethod.SAMPLE_AES;
            case "SAMPLE-AES-CTR":
                return EncryptMethod.SAMPLE_AES_CTR;
            case "CENC":
                return EncryptMethod.CENC;
            case "CHACHA20":
                return EncryptMethod.CHACHA20;
            default:
                return EncryptMethod.UNKNOWN;
        }
    }


    public static String GetEncryptMethodString(EncryptMethod method)
    {
        switch (method)
        {
            case EncryptMethod.AES_128:
                return "AES-128";
                case EncryptMethod.AES_128_ECB:
                return "AES-128-ECB";
                case EncryptMethod.SAMPLE_AES:
                return "SAMPLE-AES";
                case EncryptMethod.SAMPLE_AES_CTR:
                return "SAMPLE-AES-CTR";
                case EncryptMethod.CENC:
                return "CENC";
                case EncryptMethod.CHACHA20:
                return "CHACHA20";
                default:
                return "UNKNOWN";
        }
    }

    public static EncryptMethod GetEncryptMethod(int method)
    {
        switch (method)
        {
            case 0:
                return EncryptMethod.NONE;
            case 1:
                return EncryptMethod.AES_128;
            case 2:
                return EncryptMethod.AES_128_ECB;
            case 3:
                return EncryptMethod.SAMPLE_AES;
            case 4:
                return EncryptMethod.SAMPLE_AES_CTR;
            case 5:
                return EncryptMethod.CENC;
            case 6:
                return EncryptMethod.CHACHA20;
            default:
                return EncryptMethod.UNKNOWN;
        }
    }
}