using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibrarySystem.Api.Data.Configurations;

public class BorrowerConfiguration
    : IEntityTypeConfiguration<Borrower>
{
    public void Configure(EntityTypeBuilder<Borrower> builder)
    {
        builder.ToTable("Borrowers");

        builder.HasKey(borrower => borrower.Id);

        builder.Property(borrower => borrower.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(borrower => borrower.MembershipDate)
            .IsRequired();

        builder.HasData(
            new
            {
                Id = 1,
                Name = "Mohamed Ali",
                MembershipDate = new DateTime(2025, 1, 10)
            },
            new
            {
                Id = 2,
                Name = "Mona Hassan",
                MembershipDate = new DateTime(2025, 3, 1)
            }
        );
    }
}