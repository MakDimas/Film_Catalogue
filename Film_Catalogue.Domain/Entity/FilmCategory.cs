# nullable disable

namespace Film_Catalogue.Domain.Entity
{
    public class FilmCategory
    {
        public int Id { get; set; }

        public int FilmId { get; set; }

        public int CategoryId { get; set; }

        // Додав дане поле для зв'язку з таблицею "Film"
        public Film Film { get; set; }

        // Додав дане поле для зв'язку з таблицею "Category"
        public Category Category { get; set; }
    }
}