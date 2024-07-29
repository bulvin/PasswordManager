namespace PasswordManager.Data
{
    public class Pass
    {
        public Guid Id { get; init; }
        public Uri WebsiteUrl { get; set; } = null!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string IV { get; set; } = default!;
        public User User { get; set; } = null!;
        public Guid UserId { get; init; }

    }
}
