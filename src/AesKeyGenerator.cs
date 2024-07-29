using System.Text.Json.Nodes;
using System.Text.Json;
using System.Security.Cryptography;

namespace PasswordManager
{
    public static class AesKeyGenerator
    {
        public static void GenerateKeyAndStoreInAppSettings()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();

            var key = Convert.ToBase64String(aes.Key);
           
            StoreKeyInAppSettings(key);
        }
        private static void StoreKeyInAppSettings(string key)
        {
            var filePath = "appsettings.development.json";
            var json = File.ReadAllText(filePath);
            var jsonObject = JsonSerializer.Deserialize<JsonObject>(json);

            if (jsonObject != null)
            {
                var encryptionSection = jsonObject["Encryption"]?.AsObject();
                if (encryptionSection == null)
                {
                    encryptionSection = new JsonObject();
                    jsonObject["Encryption"] = encryptionSection;
                }

                encryptionSection["Key"] = key;

                var updatedJson = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);
            }
        }
    }
}
