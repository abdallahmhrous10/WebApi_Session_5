using LibrarySystem.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Models
{

    public class Book: IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        public int AuthorId { get; set; }

        public Author Author { get; set; } = null!;

        public ICollection<Loan> Loans { get; set; }
            = new List<Loan>();
    }
}
