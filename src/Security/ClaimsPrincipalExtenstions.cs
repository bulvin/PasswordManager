using System.Security.Claims;

namespace PasswordManager.Security
{
    public static class ClaimsPrincipalExtenstions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (!Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            {
                throw new InvalidOperationException("Invalid UserId");
            }

            return id;
        }
    }
}
