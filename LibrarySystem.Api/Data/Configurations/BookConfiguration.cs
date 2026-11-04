using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibrarySystem.Api.Data.Configurations;

public class BookConfiguration
    : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(book => book.ISBN)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(book => book.ISBN)
            .IsUnique();

        builder.HasOne(book => book.Author)
            .WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new
            {
                Id = 1,
                Title = "C# Fundamentals",
                ISBN = "9780000000001",
                AuthorId = 1
            },
            new
            {
                Id = 2,
                Title = "Learning ASP.NET Core",
                ISBN = "9780000000002",
                AuthorId = 2
            }
        );
    }
}