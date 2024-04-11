using Film_Catalogue.Domain.Entity;
using Film_Catalogue.Domain.Response;
using Film_Catalogue.Domain.ViewModel.Filter;

namespace Film_Catalogue.Service.Interfaces
{
    public interface IFilmService
    {
        IBaseResponse<IQueryable<Film>> GetFilms();

        Task<IBaseResponse<Film>> CreateFilm(Film film, int[] categories);

        IBaseResponse<IQueryable<Film>> FilteredFilms(FilterViewModel model);

        IBaseResponse<Film> GetFilm(int id);

        Task<IBaseResponse<Film>> RemoveFilm(int id);

        Task<IBaseResponse<Film>> UpdateFilm(int[] categoryIds, int filmId);

        Task<IBaseResponse<Film>> UpdateAllFilm(Film filmUpdate);
    }
}