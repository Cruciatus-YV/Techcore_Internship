using Microsoft.EntityFrameworkCore;
using Techcore_Internship.AuthorsApi.Data.Interfaces;
using Techcore_Internship.AuthorsApi.Domain;

namespace Techcore_Internship.AuthorsApi.Data.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AuthorsDbContext _dbContext;
        private readonly DbSet<AuthorEntity> _DbSet;

        public AuthorRepository(AuthorsDbContext dbContext)
        {
            _dbContext = dbContext;
            _DbSet = _dbContext.Authors;
        }

        public async Task<Guid> CreateAsync(AuthorEntity author)
        {
            _DbSet.Add(author);

            await _dbContext.SaveChangesAsync(); 
            
            return author.Id;
        }

        public async Task<List<AuthorEntity>> GetAllAsync()
        {
            var authors = await _DbSet.AsNoTracking().ToListAsync();

            return authors;
        }

        public Task<AuthorEntity?> GetByIdAsync(Guid id)
        {
            var author = _DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            return author;
        }

        public async Task<bool> UpdateAsync(AuthorEntity author)
        {
            _DbSet.Update(author);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var author = _DbSet.FirstOrDefault(x => x.Id == id);
            if (author == null)
            {
                return false;
            }

            _DbSet.Remove(author);

            return await _dbContext.SaveChangesAsync() > 0;

        }
    }
}
