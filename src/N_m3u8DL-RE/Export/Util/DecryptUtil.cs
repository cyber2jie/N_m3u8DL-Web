using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Common.Util;
using N_m3u8DL_RE.Crypto;

using System.Security.Cryptography;



namespace N_m3u8DL_RE.Export.Util
{


    public class DecryptKey
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }
    public class DecryptUtil
    {


        public static  async Task<byte[]?> Decrypt(byte[] content,EncryptMethod method,DecryptKey decryptKey)
        {

                byte[] keyBytes = HexUtil.HexToBytes(decryptKey.Key);
                byte[] ivBytes = HexUtil.HexToBytes(decryptKey.IV);

                switch (method)
                {
                    case EncryptMethod.AES_128:
                        {
                            content=AES128Decrypt(content, keyBytes, ivBytes);
                            break;
                        }
                    case EncryptMethod.AES_128_ECB:
                        {
                   
                            content=AES128Decrypt(content, keyBytes, ivBytes, System.Security.Cryptography.CipherMode.ECB);
                            break;
                        }
                    case EncryptMethod.CHACHA20:
                        {
                        
                            content = ChaCha20Util.DecryptPer1024Bytes(content,keyBytes, ivBytes);
                            break;
                        }
                    case EncryptMethod.SAMPLE_AES_CTR:
                        
                        break;
                }

            return content;
        }



        public static byte[] AES128Decrypt(byte[] encryptedBuff, byte[] keyByte, byte[] ivByte, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            byte[] inBuff = encryptedBuff;

            Aes dcpt = Aes.Create();
            dcpt.BlockSize = 128;
            dcpt.KeySize = 128;
            dcpt.Key = keyByte;
            dcpt.IV = ivByte;
            dcpt.Mode = mode;
            dcpt.Padding = padding;

            ICryptoTransform cTransform = dcpt.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inBuff, 0, inBuff.Length);
            return resultArray;
        }
    }
}
