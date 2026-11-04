using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.Api.Identity
{
    public class IdentitySeeder
    {

        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            string[] roles =
                [
                    "Admin",
                        "Librarian",
                        "Member"
                ];


            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(

                        new ApplicationRole
                        {
                            Name = role
                        }
                        );

                    if (!result.Succeeded)
                    {

                        throw new InvalidOperationException($"Could not create role {role}: " +
                        string.Join(", ",
                            result.Errors.Select(error => error.Description)));

                    }
                }
            }

            var Email = configuration["SeedAdmin:Email"];

            var UserName = configuration["SeedAdmin:UserName"];

            var Password = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(Email) &&
                string.IsNullOrWhiteSpace(UserName) &&
                string.IsNullOrWhiteSpace(Password))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(Password))
            {
                throw new InvalidOperationException(
                    "All SeedAdmin settings must be provided together.");
            }


            var admin = await userManager.FindByEmailAsync(Email);

            if(admin is null)
            {
                admin = new ApplicationUser
                {

                    FullName = "System Administrator",
                    UserName = UserName,
                    Email = Email,
                    EmailConfirmed = true
                };

                var CreationalResult = await userManager.CreateAsync(admin, Password);

                if(!CreationalResult.Succeeded)
                {
                    throw new InvalidOperationException(
                 "Could not create initial Admin: " +
                 string.Join(", ",
                     CreationalResult.Errors.Select(error => error.Description)));
                }
            }

            if(!await userManager.IsInRoleAsync(admin,"Admin"))
            {
                var roleResult =await userManager.AddToRoleAsync(admin, "Admin");

                if(!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                "Could not assign Admin role: " +
                string.Join(", ",
                    roleResult.Errors.Select(error => error.Description)));
                }
            }

        }
    }
}
