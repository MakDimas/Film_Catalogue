#nullable disable
using Film_Catalogue.DAL.Interfaces;
using Film_Catalogue.Domain.Entity;
using Film_Catalogue.Domain.Response;
using Film_Catalogue.Domain.ViewModel.Filter;
using Film_Catalogue.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Film_Catalogue.Service.Implementations
{
    public class FilmService : IFilmService
    {
        private readonly ILogger<FilmService> _logger;
        private readonly IBaseRepository<Film> _filmRepo;
        private readonly IBaseRepository<Category> _categoryRepo;

        public FilmService(ILogger<FilmService> logger, IBaseRepository<Film> filmRepo, IBaseRepository<Category> categorymRepo)
        {
            _logger = logger;
            _filmRepo = filmRepo;
            _categoryRepo = categorymRepo;
        }

        // Отримання всіх фільмів
        public IBaseResponse<IQueryable<Film>> GetFilms()
        {
            try
            {
                _logger.LogInformation("[FilmService: GetFilms]: The method is started: {Now}", DateTime.Now);

                var films = _filmRepo.GetAll().Include(x => x.FilmCategories).ThenInclude(x => x.Category);

                if(films == null)
                {
                    _logger.LogInformation("[FilmService: GetFilms]: Film is null: {Now}", DateTime.Now);

                    return new BaseResponse<IQueryable<Film>>()
                    {
                        Description = "Films is null",
                        StatusCode = Domain.Enum.StatusCode.FilmNotFound
                    };
                }

                _logger.LogInformation("[FilmService: GetFilms]: The method is succeed");

                return new BaseResponse<IQueryable<Film>>()
                {
                    Data = films,
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[FilmService: GetFilms]: Error: {Message}", e.Message);

                return new BaseResponse<IQueryable<Film>>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Створення фільму
        public async Task<IBaseResponse<Film>> CreateFilm(Film addFilm, int[] categories)
        {
            try
            {
                _logger.LogInformation("[FilmService: CreateFilm]: The method is started: {Now}", DateTime.Now);

                var filmCategories = new List<FilmCategory>();
                foreach(var category in categories)
                {
                    filmCategories.Add(new FilmCategory { CategoryId = category });
                }

                var film = new Film()
                {
                    Name = addFilm.Name,
                    Director = addFilm.Director,
                    Release = addFilm.Release,
                    FilmCategories = filmCategories
                };

                await _filmRepo.Create(film);

                _logger.LogInformation("[FilmService: GetFilms]: The metod is succeed: {Now}", DateTime.Now);

                return new BaseResponse<Film>()
                {
                    Description = "Film was added",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[FilmService: GetFilms]: Error: {Messsage}", e.Message);

                return new BaseResponse<Film>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Фільтрування фільму за категорією, режисером та роком випуску
        public IBaseResponse<IQueryable<Film>> FilteredFilms(FilterViewModel model)
        {
            try
            {
                _logger.LogInformation("[FilmService: FilteredFilm]: The method is started: {Now}", DateTime.Now);

                var films = _filmRepo.GetAll().Include(x => x.FilmCategories).ThenInclude(x => x.Category).AsQueryable();

                if(films == null)
                {
                    _logger.LogInformation("[FilmService: FilteredFilm]: Film was null: {Now}", DateTime.Now);

                    return new BaseResponse<IQueryable<Film>>()
                    {
                        Description = "Film was null",
                        StatusCode = Domain.Enum.StatusCode.FilmNotFound
                    };
                }

                
                films = films.Where(x =>
                    (model.Years == null || model.Years.Contains(x.Release.Year)) &&
                    (model.Directors == null || model.Directors.Contains(x.Director)) &&
                    (model.Categories == null || x.FilmCategories.All(c => model.Categories.Contains(c.CategoryId))));

                if(model.Categories != null)
                {
                    films = films.Where(x => x.FilmCategories.Any());
                }

                _logger.LogInformation("[FilmService: FilteredFilm]: The metod is succeed: {Now}", DateTime.Now);

                return new BaseResponse<IQueryable<Film>>()
                {
                    Data = films,
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[FilmService: FilteredFilm]: {Message}", e.Message);

                return new BaseResponse<IQueryable<Film>>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Отримання обранного фільму
        public IBaseResponse<Film> GetFilm(int id)
        {
            try
            {
                _logger.LogInformation("[FilmService: GetFilm]: The method is started: {Now}", DateTime.Now);

                var film = _filmRepo.GetAll().Include(x => x.FilmCategories).ThenInclude(x => x.Category).FirstOrDefault(x => x.Id == id);

                if(film == null)
                {
                    _logger.LogInformation("[FilmService: GetFilm]: Film is null: {Now}", DateTime.Now);

                    return new BaseResponse<Film>()
                    {
                        Description = "Film is null",
                        StatusCode = Domain.Enum.StatusCode.FilmNotFound
                    };
                }

                _logger.LogInformation("[FilmService: GetFilm]: The method is succeed: {Now}", DateTime.Now);

                return new BaseResponse<Film>()
                {
                    Data = film,
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[FilmService: GetFilm]: Error: {Message}", e.Message);

                return new BaseResponse<Film>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Видалення фільму
        public async Task<IBaseResponse<Film>> RemoveFilm(int id)
        {
            try
            {
                _logger.LogInformation("[FilmService: RemoveFilm]: The method is started: {Now}", DateTime.Now);
                
                var film = _filmRepo.GetAll().Include(x => x.FilmCategories).ThenInclude(x => x.Category).FirstOrDefault(x => x.Id == id);

                if (film == null)
                {
                    _logger.LogInformation("[FilmService: RemoveFilm]: Film is null: {Now}", DateTime.Now);

                    return new BaseResponse<Film>()
                    {
                        Description = "Film is null",
                        StatusCode = Domain.Enum.StatusCode.FilmNotFound
                    };
                }

                await _filmRepo.Delete(film);

                _logger.LogInformation("[FilmService: RemoveFilm]: The method is succeed: {Now}", DateTime.Now);

                return new BaseResponse<Film>()
                {
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[FilmService: RemoveFilm]: Error: {Message}", e.Message);

                return new BaseResponse<Film>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Оновлення категорій фільму (окремо виділив оновлення категорій)
        public async Task<IBaseResponse<Film>> UpdateFilm(int[] categoryIds, int filmId)
        {
            try
            {
                _logger.LogInformation("[FilmService: UpdateFilm]: The method started: {Now}", DateTime.Now);

                var film = _filmRepo.GetAll().Include(x => x.FilmCategories).ThenInclude(x => x.Category).FirstOrDefault(x => x.Id == filmId);

                if(film == null)
                {
                    _logger.LogInformation("[FilmService: UpdateFilm]: Film is null: {Now}", DateTime.Now);

                    return new BaseResponse<Film>()
                    {
                        Description = "Film is null",
                        StatusCode = Domain.Enum.StatusCode.FilmNotFound
                    };
                }

                var filmCategories = new List<FilmCategory>();
                foreach (var category in categoryIds)
                {
                    filmCategories.Add(new FilmCategory { Category = _categoryRepo.GetAll().FirstOrDefault(x => x.Id == category) });
                }

                film.FilmCategories = filmCategories;

                await _filmRepo.Update(film);

                _logger.LogInformation("[FilmService: UpdateFilm]: The method succeed: {Now}", DateTime.Now);

                return new BaseResponse<Film>()
                {
                    Data = film,
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "[FilmService: UpdateFilm]: Error: {Message}", e.Message);

                return new BaseResponse<Film>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }

        // Оновлення даних фільму (крім категорій)
        public async Task<IBaseResponse<Film>> UpdateAllFilm(Film filmUpdate)
        {
            try
            {
                _logger.LogInformation("[FilmService: UpdateAllFilm]: The method is started: {Now}", DateTime.Now);

                var film = _filmRepo.GetAll().Include(x => x.FilmCategories).ThenInclude(x => x.Category).FirstOrDefault(x => x.Id == filmUpdate.Id);

                if(film == null)
                {
                    _logger.LogInformation("[FilmService: UpdateFilm]: Film is null: {Now}", DateTime.Now);

                    return new BaseResponse<Film>()
                    {
                        Description = "Film is null",
                        StatusCode = Domain.Enum.StatusCode.FilmNotFound
                    };
                }

                film.Name = filmUpdate.Name;
                film.Director = filmUpdate.Director;
                film.Release = filmUpdate.Release;

                await _filmRepo.Update(film);

                _logger.LogInformation("[FilmService: UpdateFilm]: The method is succeed: {Now}", DateTime.Now);

                return new BaseResponse<Film>()
                {
                    Data = film,
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch(Exception e)
            {
                _logger.LogInformation(e, "[FilmService: UpdateFilm]: {Message}", e.Message);

                return new BaseResponse<Film>()
                {
                    Description = e.Message,
                    StatusCode = Domain.Enum.StatusCode.InternalServerError
                };
            }
        }
    }
}