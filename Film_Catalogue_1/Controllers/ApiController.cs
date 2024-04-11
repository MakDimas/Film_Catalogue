using Microsoft.AspNetCore.Mvc;
using Film_Catalogue.Service.Interfaces; 

namespace Film_Catalogue.Controllers.Api
{
    [Route("Api/GetAllCategories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Метод для заповнення категорій фільму
        public IActionResult GetAllCategories()
        {
            var categories = _categoryService.GetCategories();

            if(categories.StatusCode == Domain.Enum.StatusCode.OK)
            {
                var categoriesJson = categories.Data?.Select(c => new { c.Id, c.Name });
                return Ok(categoriesJson);
            }

            return BadRequest();
        }
    }
}