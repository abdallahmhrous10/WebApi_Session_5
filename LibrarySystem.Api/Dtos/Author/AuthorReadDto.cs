using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Dtos.Author
{
    public class AuthorReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }
}
