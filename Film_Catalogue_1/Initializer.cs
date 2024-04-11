using Film_Catalogue.DAL.Interfaces;
using Film_Catalogue.DAL.Repositories;
using Film_Catalogue.Domain.Entity;
using Film_Catalogue.Service.Implementations;
using Film_Catalogue.Service.Interfaces;

namespace Film_Catalogue
{
    public static class Initializer
    {
        // Додавання репозиторіїв
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<Film>, FilmRepository>();
            services.AddScoped<IBaseRepository<Category>, CategoryRepository>();
            services.AddScoped<IBaseRepository<FilmCategory>, FilmCategoryRepository>();
        }

        // Додавання сервісів
        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IFilmService, FilmService>();
            services.AddScoped<ICategoryService, CategoryService>();
        }
    }
}