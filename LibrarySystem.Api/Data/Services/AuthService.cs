using LibrarySystem.Api.Authentication;
using LibrarySystem.Api.Common;
using LibrarySystem.Api.Dtos.Auth;
using LibrarySystem.Api.Identity;
using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.Api.Data.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtTokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IJwtTokenService tokenService,
        ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _logger = logger;
        }

       public async Task<Result<bool>> AssignRole(AssignRoleDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return Result<bool>.NotFound(
                    "User was not found.");
            }

            var requestedRole = dto.Role.Trim();

            var role = new[] { "Librarian", "Member" }
            .FirstOrDefault(item =>

            string.Equals(

                item,
                requestedRole,
                StringComparison.OrdinalIgnoreCase
                )

            );

            if (role is null)
            {
                return Result<bool>.Failure(
                    "Only Librarian and Member roles can be assigned.");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return Result<bool>.Failure(
                    $"Role {role} does not exist.");
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                return Result<bool>.Success(
                    true,
                    $"User already has the {role} role.");
            }

            var result = await _userManager.AddToRoleAsync(
                user,
                role);

            if (!result.Succeeded)
            {
                return Result<bool>.Failure(
                    "Could not assign role.",
                    result.Errors
                        .Select(error => error.Description)
                        .ToArray());
            }

            _logger.LogInformation(
                "Role {Role} was assigned to user {UserId}.",
                role,
                user.Id);

            return Result<bool>.Success(
                true,
                $"{role} role assigned successfully.");

        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return Result<AuthResponseDto>.Unauthorized(
                    "Invalid email or password.");
            }

            var signInResult =
                await _signInManager.CheckPasswordSignInAsync(
                    user,
                    dto.Password,
                    lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                return Result<AuthResponseDto>.Unauthorized(
                    "Your account is temporarily locked.");
            }

            if (!signInResult.Succeeded)
            {
                return Result<AuthResponseDto>.Unauthorized(
                    "Invalid email or password.");
            }

            _logger.LogInformation(
                "User {UserId} logged in successfully.",
                user.Id);

            return await CreateAuthResponseAsync(
                user,
                "Login completed successfully.");
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var ExistingUserName = await _userManager.FindByNameAsync(dto.UserName);

            if (ExistingUserName is not null)
            {
                return Result<AuthResponseDto>.Conflict("user name is already taken");
            }


            var ExistingEmail = await _userManager.FindByNameAsync(dto.Email);

            if (ExistingEmail is not null)
            {
                return Result<AuthResponseDto>.Conflict("user email is already taken");
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.UserName,
            };
        

            var CreationResult = await _userManager.CreateAsync(user, dto.Password);

            if(! CreationResult.Succeeded )
            {
                var errors = CreationResult.Errors.Select(error => error.Description).ToArray();

                return Result<AuthResponseDto>.Failure(
                          "User registration failed.",
                            errors);

            }



            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                var errors = roleResult.Errors
                    .Select(error => error.Description)
                    .ToArray();

                return Result<AuthResponseDto>.Failure(
                    "Could not assign the default Member role.",
                    errors);
            }

            _logger.LogInformation(
             "User {UserId} registered successfully.",
              user.Id);

              return await CreateAuthResponseAsync(
                user,
                "Registration completed successfully.",
                201);
        }


        private async Task<Result<AuthResponseDto>> CreateAuthResponseAsync(
         ApplicationUser user,
         string message,
         int statusCode = 200)
        {
            var tokenResult = await _tokenService
                .CreateTokenAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var response = new AuthResponseDto
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                AccessToken = tokenResult.AccessToken,
                ExpiresAtUtc = tokenResult.ExpiresAtUtc,
                Roles = roles
            };

            return Result<AuthResponseDto>.Success(
                response,
                message,
                statusCode);
        }
    }
}
