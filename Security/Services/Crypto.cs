using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace PasswordManager.Security.Encryption
{
    public class CryptOptions
    {
        public required byte[] Key { get; init; }
        public required byte[] IV { get; init; }
    }

    public class Crypto(IOptions<CryptOptions> options)
    {
        public string EncryptString(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = options.Value.Key;
            aes.IV = options.Value.IV;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(plainText);

            return Convert.ToBase64String(ms.ToArray());

        }

        public string DecryptString(string cipherText) {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aesAlg = Aes.Create();
            aesAlg.Key = options.Value.Key;
            aesAlg.IV = options.Value.IV;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(fullCipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();

        }

    }

}
