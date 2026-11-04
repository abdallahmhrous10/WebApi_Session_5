using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibrarySystem.Api.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LibrarySystem.Api.Authentication
{
    public class JwtTokenService : IJwtTokenService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }
        public async Task<(string AccessToken, DateTime ExpiresAtUtc)>
            CreateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
{
    new Claim(
        JwtRegisteredClaimNames.Sub,
        user.Id.ToString()),

    new Claim(
        JwtRegisteredClaimNames.Jti,
        Guid.NewGuid().ToString()),

    new Claim(
        JwtRegisteredClaimNames.Email,
        user.Email ?? string.Empty),

    new Claim(
        JwtRegisteredClaimNames.UniqueName,
        user.UserName ?? string.Empty)
};

            claims.AddRange(
                roles.Select(role =>
                    new Claim("role", role)));

            claims.AddRange(
                roles.Select(role =>
                    new Claim(ClaimTypes.Role, role)));



            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(
                _jwtOptions.ExpirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return (accessToken, expiresAt);

        }

      
    }
}
