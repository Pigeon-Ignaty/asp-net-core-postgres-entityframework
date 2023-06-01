using kinological_club.Tables;
using kinological_club.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using kinological_club.Migrations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using Microsoft.AspNetCore.Authorization;

namespace kinological_club.Controllers
{
    public class AwardsController : Controller
    {
        private readonly DogsDataContext context;

        public AwardsController(DogsDataContext context)
        {
            this.context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var awards = context.Award.ToList();
            var awardViewModels = awards.Select(award => new AwardsViewModel
            {
                Id = award.Id,
                DogId = award.DogId,
                Name = award.Name,
                DateOfReceiving = award.DateOfReceiving
            }).ToList();
            ViewBag.Role = TempData["Role"] as string;
            TempData.Keep("Role");
            return View(awardViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("awards/create")]
        public IActionResult Create(AwardsViewModel model)
        {
            ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
            try
            {
                if (ModelState.IsValid)
                {
                    var dog = context.Dogs.Find(model.DogId);
                    if (dog == null)
                    {
                        ModelState.AddModelError("DogId", "Выберите существующую собаку.");
                        ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
                        return View(model);
                    }

                    var award = new Award
                    {
                        DogId = model.DogId,
                        Name = model.Name,
                        DateOfReceiving = model.DateOfReceiving?.ToUniversalTime()//
                    };

                    context.Award.Add(award);
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                Exception innerException = ex.InnerException;

                if (innerException is InvalidCastException)
                {
                    ModelState.AddModelError("", "Дата получения награды не может быть больше текущей даты.");
                    ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
                    return View(model);
                }
                else if (innerException is PostgresException postgresException)
                {
                    // Проверяем код ошибки PostgresException
                    if (postgresException.SqlState == "P0001")
                    {
                        ModelState.AddModelError("DateOfReceiving", "Дата получения награды не может быть больше текущей даты.");
                    }
                }


/*                ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
*/                return View(model);
            }



            ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var award = context.Award.Find(id);
            if (award == null)
            {
                return NotFound();
            }

            var dogs = context.Dogs.ToList();
            ViewBag.DogsList = new SelectList(dogs, "Id", "Nickname", award.DogId);

            var awardViewModel = new AwardsViewModel
            {
                Id = award.Id,
                DogId = award.DogId,
                Name = award.Name,
                DateOfReceiving = award.DateOfReceiving?.ToUniversalTime()//
            };

            return View(awardViewModel);
        }


        [HttpPost]
        public IActionResult Edit(AwardsViewModel awardViewModel)
        {
            try { 
                if (ModelState.IsValid)
                {
                    var award = new Award
                    {
                        Id = awardViewModel.Id,
                        DogId = awardViewModel.DogId,
                        Name = awardViewModel.Name,
                        DateOfReceiving = awardViewModel.DateOfReceiving?.ToUniversalTime()

                };

                    context.Entry(award).State = EntityState.Modified;
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                Exception innerException = ex.InnerException;

                if (innerException is PostgresException postgresException)
                {
                    // Проверяем код ошибки PostgresException
                    if (postgresException.SqlState == "P0001")
                    {
                        ModelState.AddModelError("", "Дата получения награды не может быть больше текущей даты");
                        ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
                        return View(awardViewModel);
                    }
                }

                // Если код ошибки не соответствует ожидаемому, отобразить общее сообщение об ошибке
                ModelState.AddModelError("", "Произошла ошибка при сохранении изменений. Пожалуйста, попробуйте еще раз или обратитесь в службу поддержки.");

                // Логирование ошибки
                // ...
                ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
                return View(awardViewModel);
            }
            ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
            return View(awardViewModel);
        }




        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var award = context.Award.FirstOrDefault(x => x.Id == id);
                if (award == null)
                {
                    return NotFound();
                }
                context.Award.Remove(award);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok(); // Возвращает успешный статус HTTP
        }


    }
}
