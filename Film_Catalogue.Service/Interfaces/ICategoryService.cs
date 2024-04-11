using Film_Catalogue.Domain.Entity;
using Film_Catalogue.Domain.Response;

namespace Film_Catalogue.Service.Interfaces
{
    public interface ICategoryService
    {
        IBaseResponse<IQueryable<Category>> GetCategories();

        Task<IBaseResponse<Category>> CreateCategory(Category addCategory);

        Task<IBaseResponse<Category>> Remove(int idCategory);

        Task<IBaseResponse<Category>> UpdateCategory(Category updateCategory);

        Task<int> CategoryParentLevel(Category category);

        Task<Category> GetParentCategory(Category parentCategory);

        Task<List<Category>> CategoryDaughter(Category category);

        Task<Category> GetDaughterCategories(Category parentCategory);
    }
}