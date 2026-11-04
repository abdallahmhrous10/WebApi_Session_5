using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Dtos.Books
{
    public class UpdateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int AuthorId { get; set; }
    }
}
