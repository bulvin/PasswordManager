namespace PasswordManager.Data
{
    public class User
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public string Password { get; set; } = default!;
        public List<Pass> Passes { get; set; } = [];

    }
}
