using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Responses;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Responses;
using Techcore_Internship.Data.Repositories.Dapper.Interfaces;

namespace Techcore_Internship.Data.Repositories.Dapper;

public class BookDapperRepository(IConfiguration configuration) : BaseDapperRepository(configuration), IBookDapperRepository
{
    public async Task<List<BookResponse>> GetAllWithAuthorsAsync(CancellationToken cancellationToken)
    {
        var sql = @"
        SELECT 
            b.""Id"" AS BookId,
            b.""Title"", 
            b.""Year"",
            a.""Id"" AS AuthorId,
            a.""FirstName"",
            a.""LastName""
        FROM ""Books"" b
        LEFT JOIN ""AuthorEntityBookEntity"" ba ON b.""Id"" = ba.""BooksId""
        LEFT JOIN ""Authors"" a ON ba.""AuthorsId"" = a.""Id"" AND a.""IsDeleted"" = false
        WHERE b.""IsDeleted"" = false
        ORDER BY b.""Id"", a.""Id""";

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var items = await connection.QueryAsync<BookAuthorJoinResult>(sql);

        return items
            .GroupBy(x => new { x.BookId, x.Title, x.Year })
            .Select(g =>
            {
                var authors = g.Where(x => x.AuthorId != Guid.Empty)
                              .Select(x => new AuthorReferenceResponse(x.AuthorId, x.FirstName, x.LastName))
                              .ToList();

                return new BookResponse(g.Key.BookId, g.Key.Title, g.Key.Year, authors);
            })
            .ToList();
    }

    public async Task<BookResponse?> GetByIdWithAuthorsAsync(Guid id, CancellationToken cancellationToken)
    {
        var sql = @"
        SELECT 
            b.""Id"" AS BookId,
            b.""Title"", 
            b.""Year"",
            a.""Id"" AS AuthorId,
            a.""FirstName"",
            a.""LastName""
        FROM ""Books"" b
        LEFT JOIN ""AuthorEntityBookEntity"" ba ON b.""Id"" = ba.""BooksId""
        LEFT JOIN ""Authors"" a ON ba.""AuthorsId"" = a.""Id"" AND a.""IsDeleted"" = false
        WHERE b.""Id"" = @BookId AND b.""IsDeleted"" = false
        ORDER BY b.""Id"", a.""Id""";

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var parameters = new { BookId = id };

        var items = await connection.QueryAsync<BookAuthorJoinResult>(sql, parameters);

        if (!items.Any())
            return null;

        return items
            .GroupBy(x => new { x.BookId, x.Title, x.Year })
            .Select(g =>
            {
                var authors = g.Where(x => x.AuthorId != Guid.Empty)
                              .Select(x => new AuthorReferenceResponse(x.AuthorId, x.FirstName, x.LastName))
                              .ToList();

                return new BookResponse(g.Key.BookId, g.Key.Title, g.Key.Year, authors);
            })
            .FirstOrDefault();
    }

    private class BookAuthorJoinResult
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public Guid AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
