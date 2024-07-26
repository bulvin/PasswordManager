using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Passwords.Services
{
    public interface IGenerate
    {
        string Generate(int length, bool requireNumbers, bool requireSymbols);
    }

    public class RandomPassword : IGenerate
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
