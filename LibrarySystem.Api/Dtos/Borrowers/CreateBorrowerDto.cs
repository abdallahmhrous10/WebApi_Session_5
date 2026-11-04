using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Dtos.Borrowers
{
    public class CreateBorrowerDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime MembershipDate { get; set; }
    }
}
