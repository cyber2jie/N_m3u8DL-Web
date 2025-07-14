using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Common.Util;

namespace N_m3u8DL_RE.Common.Entity;

public class EncryptInfo
{
    /// <summary>
    /// 加密方式，默认无加密
    /// </summary>
    public EncryptMethod Method { get; set; } = EncryptMethod.NONE;

    public byte[]? Key { get; set; }
    public byte[]? IV { get; set; }


    public string KeyHex
    {
        get
        {
            return Key == null ? "": HexUtil.BytesToHex(Key);
        }
    }

    public string IVHex
    {

        get
        {
            return IV == null ? "": HexUtil.BytesToHex(IV);
        }
    }

    public EncryptInfo() { }

    /// <summary>
    /// 创建EncryptInfo并尝试自动解析Method
    /// </summary>
    /// <param name="method"></param>
    public EncryptInfo(string method)
    {
        Method = ParseMethod(method);
    }

    public static EncryptMethod ParseMethod(string? method)
    {
        if (method != null && System.Enum.TryParse(method.Replace("-", "_"), out EncryptMethod m))
        {
            return m;
        }
        return EncryptMethod.UNKNOWN;
    }
}