using AutoMapper;
using LibrarySystem.Api.Common;
using LibrarySystem.Api.Data.Repositories;
using LibrarySystem.Api.Dtos.Books;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Data.Services
{
    public class BookService:GenericService<Book, BookReadDto, CreateBookDto, UpdateBookDto>, IBookService
    {

        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;

       public BookService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<BookService> logger)
        : base(bookRepository, unitOfWork, mapper, logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
        }

        public async Task<Result<PagedResult<BookReadDto>>> SearchAsync(string? searchTerm, int? authorId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _bookRepository.query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();

                query = query.Where(book =>
                    book.Title.Contains(term) ||
                    book.ISBN.Contains(term) ||
                    book.Author.Name.Contains(term));
            }

            if (authorId.HasValue)
            {
                query = query.Where(
                    book => book.AuthorId == authorId.Value);
            }

            return await GetPagedAsync(
                query,
                pageNumber,
                pageSize,
                "Books retrieved successfully.",
                cancellationToken);
        }

        public override async Task<Result<BookReadDto>> CreateAsync(
      CreateBookDto dto,
      CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetByIdAsync(
                dto.AuthorId, cancellationToken);

            if (author is null)
            {
                return Result<BookReadDto>.Failure(
                    $"Author with ID {dto.AuthorId} does not exist.");
            }

            var isbnExists = await _bookRepository.IsbnExistsAsync(
                dto.ISBN,
                cancellationToken: cancellationToken);

            if (isbnExists)
            {
                return Result<BookReadDto>.Conflict(
                    "A book with this ISBN already exists.");
            }

            return await base.CreateAsync(dto, cancellationToken);
        }

        public override async Task<Result<BookReadDto>> UpdateAsync(
            int id,
            UpdateBookDto dto,
            CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetByIdAsync(
                id, cancellationToken);

            if (book is null)
            {
                return Result<BookReadDto>.NotFound(
                    $"Book with ID {id} was not found.");
            }

            var author = await _authorRepository.GetByIdAsync(
                dto.AuthorId, cancellationToken);

            if (author is null)
            {
                return Result<BookReadDto>.Failure(
                    $"Author with ID {dto.AuthorId} does not exist.");
            }

            var duplicateIsbn = await _bookRepository.IsbnExistsAsync(
                dto.ISBN,
                excludedBookId: id,
                cancellationToken: cancellationToken);

            if (duplicateIsbn)
            {
                return Result<BookReadDto>.Conflict(
                    "Another book already uses this ISBN.");
            }

            return await base.UpdateAsync(id, dto, cancellationToken);
        }

        public override async Task<Result<bool>> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (await _bookRepository.HasLoansAsync(
                    id, cancellationToken))
            {
                return Result<bool>.Conflict(
                    "Cannot delete a book that has loan records.");
            }

            return await base.DeleteAsync(id, cancellationToken);
        }

    }
}

