using PasswordManager.AI;

namespace PasswordManager.Passwords.Services
{
    public enum PasswordType
    {
        Random = 0,
        Memorable,
    }

    public class PasswordGeneratorFactory(RandomPassword randomPassword, IGroqApiClient groqClient)
    {
        public RandomPassword RandomPassword => randomPassword;
        public IGroqApiClient GroqClient => groqClient;

        public IPasswordGeneratorStrategy GetStrategy(PasswordType type)
        {
            return type switch
            {
                PasswordType.Memorable => new MemorablePasswordGeneratorStrategy(groqClient),
                _ => new RandomPasswordGeneratorStrategy(randomPassword)
            };
        }
    }
}
