using Film_Catalogue.DAL.Interfaces;
using Film_Catalogue.Domain.Entity;

namespace Film_Catalogue.DAL.Repositories
{
    public class CategoryRepository : IBaseRepository<Category>
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;   
        }

        public IQueryable<Category> GetAll() => _context.Categories;

        public async Task Create(Category entity)
        {
            await _context.Categories.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Update(Category entity)
        {
            _context.Categories.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(Category entity)
        {
            _context.Categories.Remove(entity);

            await _context.SaveChangesAsync();
        }
    }
}