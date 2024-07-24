using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Security.Encryption
{
    public class CryptOptions
    {
        public required string Key { get; init; }
    }

    public class Crypto(IOptions<CryptOptions> options)
    {
        private readonly byte[] _key = Convert.FromBase64String(options.Value.Key);

        public (string EncryptedText, string IV) EncryptString(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            var iv = aes.IV;

            var base64IV = Convert.ToBase64String(iv);

            var encryptor = aes.CreateEncryptor(aes.Key, iv);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
            }

            var encryptedData = ms.ToArray();

            return (Convert.ToBase64String(encryptedData), base64IV);
        }

        public string DecryptString(string cipherText, string IV)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            var iv = Convert.FromBase64String(IV);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(fullCipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }

}
