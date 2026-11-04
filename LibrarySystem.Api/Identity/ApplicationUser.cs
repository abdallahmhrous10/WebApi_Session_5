using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
    }
}
