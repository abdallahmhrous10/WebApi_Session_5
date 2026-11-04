using LibrarySystem.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Models
{
    public class Borrower: IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime MembershipDate { get; set; }

        public ICollection<Loan> Loans { get; set; }
            = new List<Loan>();
    }
}
