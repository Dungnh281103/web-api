using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyAPI.Data;
using MyAPI.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyAPI.Services.Token
{
    public class TokenService
    {
        private readonly JwtTokenSettings _jwtSettings;

        public TokenService(IOptions<JwtTokenSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public TokenDto GenerateToken(AppUser user)
        {
            var claims = new[]
            {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new Claim(ClaimTypes.Name, user.UserName),
              new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
              new Claim("Nickname", user.Nickname),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          };

            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.ExpireInHours * 3600
            };
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpireInHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Don't validate lifetime for expired tokens
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
