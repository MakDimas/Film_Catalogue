using Film_Catalogue.DAL.Interfaces;
using Film_Catalogue.Domain.Entity;

namespace Film_Catalogue.DAL.Repositories
{
    public class FilmRepository : IBaseRepository<Film>
    {
        private readonly AppDbContext _context;

        public FilmRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Film> GetAll() => _context.Films;

        public async Task Create(Film entity)
        {
            await _context.Films.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Update(Film entity)
        {
            _context.Films.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(Film entity)
        {
            _context.Films.Remove(entity);

            await _context.SaveChangesAsync();
        }
    }
}