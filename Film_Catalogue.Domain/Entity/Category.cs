#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Film_Catalogue.Domain.Entity
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength (200, ErrorMessage = "Category name is too long")]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        // Додав дане поле для зв'язку з таблицею "Film_Categories"
        public ICollection<FilmCategory> FilmCategories { get; set; } = new List<FilmCategory>();
    }
}