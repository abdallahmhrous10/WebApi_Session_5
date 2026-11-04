using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Dtos.Author
{
    public class CreateAuthorDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }
}
