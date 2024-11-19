using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Models;
using TestTask.Services.Interfaces;

namespace TestTask.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;
        public AuthorService(ApplicationDbContext context) => _context = context;

        public async Task<Author> GetAuthor()
        {
            var authorWithLongestBook = await _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Books.Any()) 
                .Select(a => new
                {
                    Author = a,
                    LongestBookTitleLength = a.Books.Max(b => b.Title.Length)
                })
                .OrderByDescending(x => x.LongestBookTitleLength)
                .ThenBy(x => x.Author.Id)
                .FirstOrDefaultAsync();

            return authorWithLongestBook?.Author;
        }

        public async Task<List<Author>> GetAuthors()
        {
            DateTime cutoffDate = new DateTime(2015, 1, 1);

            var authors = await _context.Authors
                .Include(a => a.Books) 
                .Where(a => a.Books.Count(b => b.PublishDate > cutoffDate) % 2 == 0) 
                .ToListAsync();

            return authors;
        }
    }
}
