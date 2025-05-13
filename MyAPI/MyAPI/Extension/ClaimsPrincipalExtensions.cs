using System.Security.Claims;

namespace MyAPI.Extension
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId)
                ? userId
                : Guid.Empty;
        }

        public static string GetUserRole(this ClaimsPrincipal principal)
        {
            var roleClaim = principal.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? string.Empty;
        }

        public static IEnumerable<string> GetUserRoles(this ClaimsPrincipal principal)
        {
            return principal.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
        }

        public static bool IsInRole(this ClaimsPrincipal principal, string role)
        {
            return principal.Claims
                .Any(c => c.Type == ClaimTypes.Role && c.Value.Equals(role, StringComparison.OrdinalIgnoreCase));
        }
    }
}
