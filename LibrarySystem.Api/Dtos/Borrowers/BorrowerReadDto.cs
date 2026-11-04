namespace LibrarySystem.Api.Dtos.Borrowers
{
    public class BorrowerReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime MembershipDate { get; set; }
    }
}
