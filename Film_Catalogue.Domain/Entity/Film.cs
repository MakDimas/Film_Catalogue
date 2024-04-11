#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Film_Catalogue.Domain.Entity
{
    public class Film
    {
        public int Id { get; set; }

        [Required]
        [MaxLength (200, ErrorMessage = "Film name is too long")]
        public string Name { get; set; }

        [Required]
        [MaxLength (200, ErrorMessage = "Film director name is too long")]
        public string Director { get; set; }

        public DateTime Release { get; set; }

        // Додав поле FilmCategories для зв'язку з таблицею "Film_Categories"
        public ICollection<FilmCategory> FilmCategories { get; set; } = new List<FilmCategory>();
    }
}