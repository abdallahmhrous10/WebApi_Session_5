using AutoMapper;
using LibrarySystem.Api.Dtos.Author;
using LibrarySystem.Api.Dtos.Books;
using LibrarySystem.Api.Dtos.Borrowers;
using LibrarySystem.Api.Dtos.Loans;
using LibrarySystem.Api.Models;
using Microsoft.Extensions.Options;
namespace LibrarySystem.Api.Mappings
{
    public class MappingProfile:Profile
    {

        public MappingProfile()
        {
            CreateMap<CreateAuthorDto, Author>();
            CreateMap<UpdateAuthorDto,Author>();
            CreateMap<Author,AuthorReadDto>();


            CreateMap<CreateBookDto,Book>();
            CreateMap<UpdateBookDto,Book>();
            CreateMap<Book,BookReadDto>()
                .ForMember(des => des.AuthorName,
                options => options.MapFrom(src => src.Author.Name)
                );

            CreateMap<CreateBorrowerDto, Borrower>();
            CreateMap<UpdateBorrowerDto, Borrower>();
            CreateMap<Borrower, BorrowerReadDto>();

            CreateMap<CreateLoanDto, Loan>();
            CreateMap<UpdateLoanDto, Loan>();
            CreateMap<Loan, LoanReadDto>()
                .ForMember(des => des.BorrowerName,
                options => options.MapFrom(src => src.Borrower.Name))
                .ForMember(des => des.BookTitle,
                Options => Options.MapFrom(src => src.Book.Title));

        }



    }
}
