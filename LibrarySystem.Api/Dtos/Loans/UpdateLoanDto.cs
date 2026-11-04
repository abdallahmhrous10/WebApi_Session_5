using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Dtos.Loans
{
    public class UpdateLoanDto
    {
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        [Range(1, int.MaxValue)]
        public int BorrowerId { get; set; }

        public DateTime LoanDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
