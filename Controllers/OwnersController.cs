using kinological_club.Tables;
using kinological_club.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace kinological_club.Controllers
{
    public class OwnersController : Controller
    {
        private readonly DogsDataContext context;

        public OwnersController(DogsDataContext context)
        {
            this.context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var owners = context.Owners.ToList();
            var ownerViewModels = owners.Select(owner => new OwnersViewModel
            {
                OwnerId = owner.OwnerId, // Здесь используется свойство Id модели Dogs
                LastName = owner.LastName, // Здесь используется свойство Nickname модели Dogs
                FirstName = owner.FirstName, // Здесь используется свойство OwnerId модели Dogs
                MiddleName = owner.MiddleName, // Здесь используется свойство BirthDate модели Dogs
                Address = owner.Address, // Здесь используется свойство DeathDate модели Dogs
                Phone = owner.Phone, // Здесь используется свойство Gender модели Dogs
            }).ToList();
            ViewBag.Role = TempData["Role"] as string;
            TempData.Keep("Role");
            return View(ownerViewModels);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(OwnersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var owner = new Owners
                {
                    OwnerId = model.OwnerId, // Здесь используется свойство Id модели Dogs
                    LastName = model.LastName, // Здесь используется свойство Nickname модели Dogs
                    FirstName = model.FirstName, // Здесь используется свойство OwnerId модели Dogs
                    MiddleName = model.MiddleName, // Здесь используется свойство BirthDate модели Dogs
                    Address = model.Address, // Здесь используется свойство DeathDate модели Dogs
                    Phone = model.Phone, // Здесь используется свойство Gender модели Dogs
                };

                context.Owners.Add(owner);
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("Create", model);
        }
        public IActionResult Edit(int id)
        {
            var owners = context.Owners.Find(id);
            if (owners == null)
            {
                return NotFound();
            }

            var ownerViewModel = new OwnersViewModel
            {
                OwnerId = owners.OwnerId, // Здесь используется свойство Id модели Dogs
                LastName = owners.LastName, // Здесь используется свойство Nickname модели Dogs
                FirstName = owners.FirstName, // Здесь используется свойство OwnerId модели Dogs
                MiddleName = owners.MiddleName, // Здесь используется свойство BirthDate модели Dogs
                Address = owners.Address, // Здесь используется свойство DeathDate модели Dogs
                Phone = owners.Phone, // Здесь используется свойство Gender модели Dogs
            };

            return View(ownerViewModel);
        }

        [HttpPost]
        public IActionResult Edit(OwnersViewModel ownerViewModel)
        {
            if (ModelState.IsValid)
            {
                var owners = new Owners
                {
                OwnerId = ownerViewModel.OwnerId, // Здесь используется свойство Id модели Dogs
                LastName = ownerViewModel.LastName, // Здесь используется свойство Nickname модели Dogs
                FirstName = ownerViewModel.FirstName, // Здесь используется свойство OwnerId модели Dogs
                MiddleName = ownerViewModel.MiddleName, // Здесь используется свойство BirthDate модели Dogs
                Address = ownerViewModel.Address, // Здесь используется свойство DeathDate модели Dogs
                Phone = ownerViewModel.Phone, // Здесь используется свойство Gender модели Dogs
                };

                context.Entry(owners).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(ownerViewModel);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var owner = context.Owners.FirstOrDefault(x => x.OwnerId == id);
                if (owner == null)
                {
                    return NotFound();
                }
                context.Owners.Remove(owner);
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
