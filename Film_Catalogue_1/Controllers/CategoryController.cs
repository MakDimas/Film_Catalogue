using Film_Catalogue.Domain.Entity;
using Film_Catalogue.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Film_Catalogue.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Відображає всі наявні категорії на сторінці "/Category/Categories"
        public IActionResult Categories()
        {
            var categories = _categoryService.GetCategories();

            if(categories.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(categories.Data);
            }

            return StatusCode(500, categories.Description);
        }

        // Отримання обраного об'єкту Category для його зміни
        [HttpPost]
        public IActionResult GetCategory(int categoryId)
        {
            var category = _categoryService.GetCategories();

            if(category.StatusCode == Domain.Enum.StatusCode.OK)
            {
                var result = category.Data?.FirstOrDefault(x => x.Id == categoryId);
                return PartialView("_CategoryUpdatePartial", result);
            }

            return StatusCode(500, category.Description);
        }

        // Метод дії для додавання категорії
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category addCategory)
        {
            if (ModelState.IsValid)
            {
                var success = await _categoryService.CreateCategory(addCategory);

                return success.StatusCode == Domain.Enum.StatusCode.OK? 
                    View("Categories", _categoryService.GetCategories().Data)
                    : StatusCode(500, success.Description);
            }

            return StatusCode(500, "[Controller: Category; Action: AddCategory]: ModelState not valid!");
        }

        // Метод дії для видалення категорії
        [HttpPost]
        public async Task<IActionResult> RemoveCategory(int categoryId)
        {
            var removeResult = await _categoryService.Remove(categoryId);

            if(removeResult.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View("Categories", _categoryService.GetCategories().Data);
            }

            return StatusCode(500, removeResult.Description);
        }

        // Метод дії для оновлення категорії
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category updateCategory)
        {
            if (ModelState.IsValid)
            {
                var updated = await _categoryService.UpdateCategory(updateCategory);

                if(updated.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return View("Categories", _categoryService.GetCategories().Data);
                }
                else
                {
                    return StatusCode(500, updated.Description);
                }
            }

            return StatusCode(500, "[Controller: Category; Action: UpdateCategory]: ModelState not valid!");
        }
    }
}