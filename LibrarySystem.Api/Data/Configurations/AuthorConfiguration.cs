using Microsoft.EntityFrameworkCore;
using LibrarySystem.Api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LibrarySystem.Api.Data.Configurations
{
    public class AuthorConfiguration:IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author>builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(author => author.Id);

            builder.Property(author => author.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(author => author.BirthDate)
                .IsRequired();

            builder.HasData(
                new
                {
                    Id = 1,
                    Name = "Ahmed Mohamed",
                    BirthDate = new DateTime(1985, 5, 15)
                },
                new
                {
                    Id = 2,
                    Name = "Sara Ali",
                    BirthDate = new DateTime(1990, 8, 20)
                }
            );
        }
    }
}
