using PasswordManager.AI;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using static PasswordManager.Passwords.GeneratePassword;

namespace PasswordManager.Passwords.Services
{
    public interface IPasswordGeneratorStrategy
    {
        Task<string> GeneratePasswordAsync(PasswordGenerationRequest request);
    }

    public class RandomPasswordGeneratorStrategy(RandomPassword randomPassword) : IPasswordGeneratorStrategy
    {
        public Task<string> GeneratePasswordAsync(PasswordGenerationRequest request)
        {
            var password = randomPassword.Generate(request.Length, 
                request.RequireNumbers, 
                request.RequireSymbols);

            return Task.FromResult(password);
        }
    }

    public class MemorablePasswordGeneratorStrategy(IGroqApiClient groqClient) : IPasswordGeneratorStrategy
    {
        private IGroqApiClient GroqClient => groqClient;

        public async Task<string> GeneratePasswordAsync(PasswordGenerationRequest request)
        {
            var groqRequest = new JsonObject
            {
                ["model"] = "llama3-8b-8192",
                ["temperature"] = 1.5,
                ["max_tokens"] = 150,
                ["top_p"] = 0.9,
                ["frequency_penalty"] = 0.8,
                ["presence_penalty"] = 0.6,
                ["stop"] = "TERMINATE",
                ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "system",
                    ["content"] = "You are a password generator. Your task is to create unique, memorable, and secure passwords."
                },
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = $@"Generate a memorable password with the following criteria:
    - Consists of exactly {request.Length} words
    - Use complete, meaningful words: {(request.FullWords!.Value ? "Yes" : "No")}.
    - Substitute some letters in the words with symbols (e.g., 'a' with '@', 's' with '$'): {(request.RequireSymbols ? "Yes" : "No")}.    - Incorporate numbers within some words: {(request.RequireNumbers ? "Yes" : "No")}.
    - Use a mix of uncommon and common, unrelated words separated by hyphens ('-')
    - Words should come from various categories (e.g., animals, objects, actions, adjectives, foods)
    - Ensure no word is repeated
    - Respond with only the generated password, nothing else."
                }
            }
            };

            var result = await GroqClient.CreateChatCompletionAsync(groqRequest);
            var generatedPassword = result?["choices"]?[0]?["message"]?["content"]?.ToString().TrimEnd() ?? "No response found";
            return generatedPassword;
        }
    }


    public class RandomPassword
    {
        public string Generate(int length, bool requireNumbers, bool requireSymbols)
        {
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numericChars = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";
         
            var charGroups = new List<string> { lowercaseChars, uppercaseChars };

            if (requireNumbers)
            {
                charGroups.Add(numericChars);
            }

            if (requireSymbols)
            {
                charGroups.Add(specialChars);
            }

            return GenerateForProperGroups(charGroups.ToArray(), length);


        }

        private string GenerateForProperGroups(string[] charGroups, int length)
        {
            var allChars = string.Concat(charGroups);
            var password = new StringBuilder();
            int lengthPassword = length;

            foreach (var group in charGroups)
            {
                password.Append(group[RandomNumberGenerator.GetInt32(group.Length)]);
            }
            lengthPassword -= charGroups.Length;

            for (int i = 0; i < lengthPassword; i++)
            {
                password.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
            }

            char[] passwordShuffle = password.ToString().ToCharArray();

            string randomPassword = Shuffle(passwordShuffle);

            return randomPassword;


        }

        private string Shuffle(char[] password)
        {
            for (int i = password.Length - 1; i >= 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (password[i], password[j]) = (password[j], password[i]);
            }

            return new string(password);
        }

    }
}
