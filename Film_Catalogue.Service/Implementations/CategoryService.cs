#nullable disable

using Film_Catalogue.DAL.Interfaces;
using Film_Catalogue.Domain.Entity;
using Film_Catalogue.Domain.Response;
using Film_Catalogue.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Film_Catalogue.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly IBaseRepository<Category> _categoryRepo;

        public CategoryService(ILogger<CategoryService> logger, IBaseRepository<Category> categoryRepo)
        {
            _logger = logger;
            _categoryRepo = categoryRepo;
        }

        // Повернення всіх категорій
        public IBaseResponse<IQueryable<Category>> GetCategories()
        {
            try
            {
                _logger.LogInformation("[CategoriService: GetCategories]: The method is started: {Now}", DateTime.Now);

                var categories = _categoryRepo.GetAll().Include(x => x.FilmCategories).ThenInclude(x => x.Film).AsQueryable();

                if(categories == null)
                {
                    _logger.LogInformation("[CategoriService: GetCategories]: Category is null: {Now}", DateTime.Now);

                    return new BaseResponse<IQueryable<Category>>()
                    {
                        Description = "Category is null",
                        StatusCode = Domain.Enum.StatusCode.CategoryNotFound
                    };
                }

                _logger.LogInformation("[CategoriService: GetCategories]: The metod is successed: {Now}", DateTime.Now);

                return new BaseResponse<IQueryable<Category>>()
                {
                    Data = categories,
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[CategoriService: GetCategories]: Error: {Message}", e.Message);

                return new BaseResponse<IQueryable<Category>>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Створення нової категорії
        public async Task<IBaseResponse<Category>> CreateCategory(Category addCategory)
        {
            try
            {
                _logger.LogInformation("[CategoriService: CreateCategory]: The method is started: {Now}", DateTime.Now);

                var category = _categoryRepo.GetAll().Where(x => x.Name == addCategory.Name && x.ParentCategoryId == addCategory.ParentCategoryId).AsEnumerable();

                if(category.Any())
                {
                    _logger.LogInformation("[CategoriService: CreateCategory]: Category already exist: {Now}", DateTime.Now);

                    return new BaseResponse<Category>()
                    {
                        Description = "Category already exist",
                        StatusCode = Domain.Enum.StatusCode.InternalServerError
                    };
                }

                var add = new Category()
                {
                    Name = addCategory.Name,
                    ParentCategoryId = addCategory.ParentCategoryId,
                };

                await _categoryRepo.Create(add);

                _logger.LogInformation("[CategoriService: CreateCategory]: Category has been created: {Now}", DateTime.Now);

                return new BaseResponse<Category>()
                {
                    Description = "Category was added",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[CategoriService: CreateCategory]: Error: {Message}", e.Message);

                return new BaseResponse<Category>(){
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        } 

        // Видалення існуючої категорії
        public async Task<IBaseResponse<Category>> Remove(int idCategory)
        {
            try
            {
                _logger.LogInformation("[CategoriService: Remove]: The method is started: {Now}", DateTime.Now);

                var category = GetCategories().Data?.Include(x => x.FilmCategories).ThenInclude(x => x.Film).FirstOrDefault(x => x.Id == idCategory);

                if (category == null)
                {
                    _logger.LogInformation("[CategoriService: CreateCategory]: Category was null: {Now}", DateTime.Now);

                    return new BaseResponse<Category>()
                    {
                        Description = "Category was null",
                        StatusCode = Domain.Enum.StatusCode.CategoryNotFound
                    };
                }

                var daughters = GetCategories().Data?.ToList();
                daughters.Add(category);

                var daughtersList = await CategoryDaughter(category);

                // Виключення категорій ()крім тих, які видаляються
                var result = daughters.Except(daughtersList).ToList();


                foreach (var cat in result)
                {
                    await _categoryRepo.Delete(cat);
                }

                _logger.LogInformation("[CategoriService: CreateCategory]: Category has been deleted : {Now}", DateTime.Now);

                return new BaseResponse<Category>()
                {
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[CategoriService: CreateCategory]: Error: {Message}", e.Message);

                return new BaseResponse<Category>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Оновлення даних категорії
        public async Task<IBaseResponse<Category>> UpdateCategory(Category updateCategory)
        {
            try
            {
                _logger.LogInformation("[CategoriService: UpdateCategory]: The method is started: {Now}", DateTime.Now);

                if(updateCategory.ParentCategoryId == 0)
                {
                    updateCategory.ParentCategoryId = null;
                }

                var category = _categoryRepo.GetAll().Where(x => x.Name == updateCategory.Name && x.ParentCategoryId == updateCategory.ParentCategoryId).AsEnumerable();

                if (category.Any())
                {
                    _logger.LogInformation("[CategoriService: UpdateCategory]: Category already exist: {Now}", DateTime.Now);

                    return new BaseResponse<Category>()
                    {
                        Description = "Category already exist",
                        StatusCode = Domain.Enum.StatusCode.InternalServerError
                    };
                }

                await _categoryRepo.Update(updateCategory);

                _logger.LogInformation("[CategoriService: UpdateCategory]: Category was updated: {Now}", DateTime.Now);

                return new BaseResponse<Category>()
                {
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[CategoriService: UpdateCategory]: Error: {Message}", e.Message);

                return new BaseResponse<Category>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Метод для отримання рівня вкладеності категорії
        public async Task<int> CategoryParentLevel(Category category)
        {
            int level = 0;
            Category currentCategory = category;

            while (currentCategory.ParentCategoryId.HasValue)
            {
                level++;
                currentCategory = await GetParentCategory(currentCategory);
            }

            return level;
        }
        
        // Метод для отримання батьківської категорії
        public async Task<Category> GetParentCategory(Category parentCategory) => await GetCategories().Data.FirstOrDefaultAsync(x => x.Id == parentCategory.ParentCategoryId);

        // Метод для отримання категорій (крім тої, що видаляється та її дочірніх) для видалення категорії разом з її дочініми категоріями)
        public async Task<List<Category>> CategoryDaughter(Category category)
        {
            List<Category> categories = GetCategories().Data.ToList();

            Category currentCategory = category;

            while (currentCategory != null)
            {
                categories.Remove(currentCategory);
                currentCategory = await GetDaughterCategories(currentCategory);
            }

            return categories;
        }

        // Метод для отримання дочірньої категорії (якщо така є)
        public async Task<Category> GetDaughterCategories(Category parentCategory)
        {
            var categories = await GetCategories().Data.ToListAsync();

            return categories.FirstOrDefault(x => x.ParentCategoryId == parentCategory.Id);
        }
    }
}