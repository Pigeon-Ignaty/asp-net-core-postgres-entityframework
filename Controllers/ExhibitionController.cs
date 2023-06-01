using kinological_club.Tables;
using kinological_club.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using kinological_club.Migrations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authorization;

namespace kinological_club.Controllers
{
    public class ExhibitionController : Controller
    {
        private readonly DogsDataContext context;

        public ExhibitionController(DogsDataContext context)
        {
            this.context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var exhibs = context.Exhibition.ToList();
            var exhibViewModels = exhibs.Select(exhib=> new ExhibitionViewModel
            {
                Id = exhib.Id,
                DogId = exhib.DogId,
                Name = exhib.Name,
                StartDate = exhib.StartDate,
                EndDate = exhib.EndDate
            }).ToList();
            ViewBag.Role = TempData["Role"] as string;
            TempData.Keep("Role");
            return View(exhibViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("exhibition/create")]
        public IActionResult Create(ExhibitionViewModel model)
        {
            ViewBag.DogsList = new SelectList(context.Exhibition.ToList(), "Id", "Nickname");
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

                    var exhib = new Exhibition
                    {
                        DogId = model.DogId,
                        Name = model.Name,
                        StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc),
                        EndDate = model.EndDate != null ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : (DateTime?)null
                    };

                    context.Exhibition.Add(exhib);
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Произошла ошибка при сохранении изменений. Пожалуйста, попробуйте еще раз или обратитесь в службу поддержки.");

                ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
                return View(model);
            }

            ViewBag.DogsList = new SelectList(context.Dogs.ToList(), "Id", "Nickname");
            return View(model);
        }






        public IActionResult Edit(int id)
        {
            var exhib = context.Exhibition.Find(id);
            if (exhib == null)
            {
                return NotFound();
            }

            var exhibViewModel = new ExhibitionViewModel
            {
                Id = exhib.Id,
                DogId = exhib.DogId,
                Name = exhib.Name,
                StartDate = exhib.StartDate,
                EndDate = exhib.EndDate
            };

            return View(exhibViewModel);
        }

        [HttpPost]
        public IActionResult Edit(ExhibitionViewModel exhibViewModel)
        {
            if (ModelState.IsValid)
            {
                var exhib = new Exhibition
                {
                    Id = exhibViewModel.Id,
                    DogId = exhibViewModel.DogId,
                    Name = exhibViewModel.Name,
                    StartDate = DateTime.SpecifyKind(exhibViewModel.StartDate, DateTimeKind.Utc),
                    EndDate = exhibViewModel.EndDate != null ? DateTime.SpecifyKind(exhibViewModel.EndDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                };

                context.Entry(exhib).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(exhibViewModel);
        }





        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var exhib = context.Exhibition.FirstOrDefault(x => x.Id == id);
                if (exhib == null)
                {
                    return NotFound();
                }
                context.Exhibition.Remove(exhib);
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
