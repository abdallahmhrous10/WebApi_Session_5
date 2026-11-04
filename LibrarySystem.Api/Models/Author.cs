using LibrarySystem.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Models
{

    public class Author:IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public ICollection<Book> Books { get; set; }
            = new List<Book>();
    }
}
