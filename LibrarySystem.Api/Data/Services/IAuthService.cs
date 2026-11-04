using LibrarySystem.Api.Common;
using LibrarySystem.Api.Dtos.Auth;

namespace LibrarySystem.Api.Data.Services
{
    public interface IAuthService
    {

        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
       
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto Dto);

        Task<Result<bool>> AssignRole(AssignRoleDto Dto);
    }
}
