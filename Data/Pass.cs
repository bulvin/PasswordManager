namespace PasswordManager.Data
{
    public class Pass
    {
        public int Id { get; private init; }

        public Uri WebsiteUrl { get; set; } = null!;

        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;

    }
}
