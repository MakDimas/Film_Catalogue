using Film_Catalogue.Domain.Entity;
using Film_Catalogue.Domain.ViewModel.Filter;
using Film_Catalogue.Models;
using Film_Catalogue.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Film_Catalogue.Controllers
{
    public class FilmController : Controller
    {
        private readonly IFilmService _filmService;

        public FilmController(IFilmService filmservice)
        {
            _filmService = filmservice;
        }

        // Отримання усіх фільмів на сторінку "/"
        public IActionResult Films()
        {
            var films = _filmService.GetFilms();

            if(films.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(films.Data);
            }

            return StatusCode(500, films.Description);
        }

        // Метод дії для додавання фільму
        [HttpPost]
        public async Task<IActionResult> AddFilm(Film addFilm, int[] addCategory)
        {
            if (ModelState.IsValid)
            {
                var success = await _filmService.CreateFilm(addFilm, addCategory);

                if(success.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    var films = _filmService.GetFilms();
                    return View("/Views/Film/Films.cshtml", films.Data);
                }
                else
                {
                    return StatusCode(500, success.Description);
                }
            }

            return StatusCode(500, "[Controller: Film; Action: AddFilm]: ModelState not valid!");
        }

        // Метод дії для обробки фільтрів
        [HttpPost]
        public IActionResult FilmFilter(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var filtered = _filmService.FilteredFilms(filter);

                if(filtered.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return View("Films", filtered.Data);
                }
                else
                {
                    return StatusCode(500, filtered.Description);
                }
            }

            return StatusCode(500, "[Controller: Film; Action: FilmFilter]: ModelState not valid!");
        }

        // Метод дії для отримання обраного фільму
        [HttpPost]
        public IActionResult GetFilm(int id)
        {
            var film = _filmService.GetFilm(id);

            if(film.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View("SelectedFilm" ,film.Data);
            }

            return StatusCode(500, film.Description);
        }

        // Метод дії для отримання категорій фільму (для відображення у <select> з допомогою JS)
        public IActionResult SelectedFilm(int id)
        {
            var film = _filmService.GetFilm(id);

            if(film.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Json(film.Data?.FilmCategories.Select(x => new { x.Category.Id, x.Category.Name }));
            }

            return StatusCode(500, film.Description);
        }

        // Метод дії для видалення фільму
        [HttpPost]
        public async Task<IActionResult> RemoveFilm(int id)
        {
            var removeResult = await _filmService.RemoveFilm(id);

            if(removeResult.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("Films", "Film");
            }

            return StatusCode(500, removeResult.Description);
        }

        // Метод дії для оновлення категорій фільму (створений окремо, щоб підкреслити використання Api)
        [HttpPost]
        public async Task<IActionResult> UpdateFilm(int[] ids, int filmId)
        {
            var updateResult = await _filmService.UpdateFilm(ids, filmId);

            if(updateResult.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View("SelectedFilm", updateResult.Data);
            }

            return StatusCode(500, updateResult.Description);
        }

        // Метод дії для оновлення даних фільму (крім категорій)
        [HttpPost]
        public async Task<IActionResult> UpdateAllFilm(Film film)
        {
            if (ModelState.IsValid)
            {
                var updateResult = await _filmService.UpdateAllFilm(film);

                if(updateResult.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return View("SelectedFilm", updateResult.Data);
                }
                else
                {
                    return StatusCode(500, updateResult.Description);
                }
            }

            return StatusCode(500, "[Controller: Film; Action: UpdateAllFilm]: ModelState not valid!");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}