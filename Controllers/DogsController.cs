using kinological_club.Tables;
using kinological_club.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace kinological_club.Controllers
{
    public class DogsController : Controller
    {
        private readonly DogsDataContext context;

        public DogsController(DogsDataContext context)
        {
            this.context = context;
        }
        [Authorize] // Добавлен атрибут Authorize
        public IActionResult Index()
        {
            var dogs = context.Dogs.ToList();
            var dogsViewModels = dogs.Select(dog => new DogsViewModel
            {
                Id = dog.Id, // Здесь используется свойство Id модели Dogs
                Nickname = dog.Nickname, // Здесь используется свойство Nickname модели Dogs
                OwnerId = dog.OwnerId, // Здесь используется свойство OwnerId модели Dogs
                BirthDate = dog.BirthDate, // Здесь используется свойство BirthDate модели Dogs
                DeathDate = dog.DeathDate, // Здесь используется свойство DeathDate модели Dogs
                Gender = dog.Gender, // Здесь используется свойство Gender модели Dogs
                Breed = dog.Breed, // Здесь используется свойство Breed модели Dogs
                FatherId = dog.FatherId, // Здесь используется свойство FatherId модели Dogs
                MotherId = dog.MotherId, // Здесь используется свойство MotherId модели Dogs
                ExhibitionId = dog.ExhibitionId // Здесь используется свойство ExhibitionId модели Dogs
            }).ToList();
            ViewBag.Role = TempData["Role"] as string;
            TempData.Keep("Role");
            return View(dogsViewModels);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("/Dogs/Create")]
        public IActionResult Create(DogsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dogs = new Dogs
                    {
                        Nickname = model.Nickname,
                        OwnerId = model.OwnerId,
                        BirthDate = DateTime.SpecifyKind(model.BirthDate, DateTimeKind.Utc),
                        DeathDate = model.DeathDate != null ? DateTime.SpecifyKind(model.DeathDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                        Gender = model.Gender,
                        Breed = model.Breed,
                        FatherId = model.FatherId,
                        MotherId = model.MotherId,
                        ExhibitionId = model.ExhibitionId
                    };

                    context.Dogs.Add(dogs);
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                // Обработка ошибки сохранения изменений в базе данных
                // Получение дополнительной информации об ошибке из исключения ex
                // Возможно, нужно предоставить пользователю информацию о нарушении ограничения внешнего ключа

                // Пример:
                ModelState.AddModelError(string.Empty, "Ошибка сохранения данных. Пожалуйста, проверьте введенные данные и повторите попытку.");
            }

            return View("Create", model);
        }

        public IActionResult Edit(int id)
        {
            var dogs = context.Dogs.Find(id);
            if (dogs == null)
            {
                return NotFound();
            }

            var dogsViewModel = new DogsViewModel
            {
                Id = dogs.Id,
                Nickname = dogs.Nickname,
                OwnerId = dogs.OwnerId,
                BirthDate = dogs.BirthDate,
                DeathDate = dogs.DeathDate,
                Gender = dogs.Gender,
                Breed = dogs.Breed,
                FatherId = dogs.FatherId,
                MotherId = dogs.MotherId,
                ExhibitionId = dogs.ExhibitionId
            };

            return View(dogsViewModel);
        }

        [HttpPost]
        public IActionResult Edit(DogsViewModel dogsViewModel)
        {
            if (ModelState.IsValid)
            {
                var dogs = new Dogs
                {
                    Id = dogsViewModel.Id,
                    Nickname = dogsViewModel.Nickname,
                    OwnerId = dogsViewModel.OwnerId,
                    BirthDate = DateTime.SpecifyKind(dogsViewModel.BirthDate, DateTimeKind.Utc),
                    DeathDate = dogsViewModel.DeathDate != null ? DateTime.SpecifyKind(dogsViewModel.DeathDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                    Gender = dogsViewModel.Gender,
                    Breed = dogsViewModel.Breed,
                    FatherId = dogsViewModel.FatherId,
                    MotherId = dogsViewModel.MotherId,
                    ExhibitionId = dogsViewModel.ExhibitionId
                };

                context.Entry(dogs).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(dogsViewModel);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var dogs = context.Dogs.FirstOrDefault(x => x.Id == id);
                if (dogs == null)
                {
                    return NotFound();
                }
                context.Dogs.Remove(dogs);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok(); // Возвращает успешный статус HTTP
        }

        [HttpPost]
        public IActionResult Archive()
        {
            using (var connection = new NpgsqlConnection("Server=localhost;Database=dogsclub;Port=5432;User Id=postgres;Password=d1a2s3h4a5")) // Замените "your_connection_string" на свою строку подключения
            {
                connection.Open();

                using (var command = new NpgsqlCommand("CALL archive_dead_dogs()", connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }


    }
}
