using PasswordManager.Passwords.Services;
using static PasswordManager.Passwords.GenerateRandomPassword;

namespace PasswordManager.Passwords
{
    public static class PasswordGenerationRequestDefaults
    {
        public static PasswordGenerationRequest GetDefaultRequest(PasswordType type)
        {
            return type switch
            {
                PasswordType.Random => new PasswordGenerationRequest(15, false, true, false, type),
                PasswordType.Memorable => new PasswordGenerationRequest(5, false, false, true, type),
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"No defaults for password type {type}")
            };
        }
    }
}
