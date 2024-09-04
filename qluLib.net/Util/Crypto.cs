using System.Security.Cryptography;
using System.Text;

namespace qluLib.net.Util;

public static class Crypto
{
    public static string DesEncrypt(string keyBase64, string data)
    {
        var keyBytes = Convert.FromBase64String(keyBase64);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var des = DES.Create();
        des.Key = keyBytes;
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;
        using var encryptor = des.CreateEncryptor();
        var encryptedBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
        return Convert.ToBase64String(encryptedBytes);
    }
}