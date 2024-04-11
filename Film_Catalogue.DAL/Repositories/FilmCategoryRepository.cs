using Film_Catalogue.DAL.Interfaces;
using Film_Catalogue.Domain.Entity;

namespace Film_Catalogue.DAL.Repositories
{
    public class FilmCategoryRepository : IBaseRepository<FilmCategory>
    {
        private readonly AppDbContext _context;

        public FilmCategoryRepository(AppDbContext context)
        {
            _context = context;   
        }

        public IQueryable<FilmCategory> GetAll() => _context.FilmCategories;

        public async Task Create(FilmCategory entity)
        {
            await _context.FilmCategories.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Update(FilmCategory entity)
        {
            _context.FilmCategories.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(FilmCategory entity)
        {
            _context.FilmCategories.Remove(entity);

            await _context.SaveChangesAsync();
        }
    }
}