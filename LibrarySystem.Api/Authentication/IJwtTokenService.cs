using LibrarySystem.Api.Identity;

namespace LibrarySystem.Api.Authentication
{
    public interface IJwtTokenService
    {

        Task<(string AccessToken, DateTime ExpiresAtUtc)> CreateTokenAsync(ApplicationUser User);

    }
}
