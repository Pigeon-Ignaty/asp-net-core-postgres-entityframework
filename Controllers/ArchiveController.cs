using kinological_club.Tables;
using kinological_club.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using kinological_club.Migrations;
using Microsoft.AspNetCore.Authorization;

namespace kinological_club.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly DogsDataContext context;

        public ArchiveController(DogsDataContext context)
        {
            this.context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var archives = context.Archive.ToList();
            var archiveViewModels = archives.Select(archive => new ArchiveViewModel
            {
                Id = archive.Id,
                Nickname = archive.Nickname,
                OwnerId = archive.OwnerId,
                BirthDate = archive.BirthDate,
                DeathDate = archive.DeathDate,
                Gender = archive.Gender,
                Breed = archive.Breed,
                FatherId = archive.FatherId,
                MotherId = archive.MotherId,
                ExhibitionId = archive.ExhibitionId
            }).ToList();
            ViewBag.Role = TempData["Role"] as string;
            TempData.Keep("Role");
            return View(archiveViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("archive/create")]
        public IActionResult Create(ArchiveViewModel model)
        {
            if (ModelState.IsValid)
            {
                var archive = new Archive
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

                context.Archive.Add(archive);
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var archive = context.Archive.Find(id);
            if (archive == null)
            {
                return NotFound();
            }

            var archiveViewModel = new ArchiveViewModel
            {
                Id = archive.Id,
                Nickname = archive.Nickname,
                OwnerId = archive.OwnerId,
                BirthDate = archive.BirthDate,
                DeathDate = archive.DeathDate,
                Gender = archive.Gender,
                Breed = archive.Breed,
                FatherId = archive.FatherId,
                MotherId = archive.MotherId,
                ExhibitionId = archive.ExhibitionId
            };

            return View(archiveViewModel);
        }

        [HttpPost]
        public IActionResult Edit(ArchiveViewModel archiveViewModel)
        {
            if (ModelState.IsValid)
            {
                var archive = new Archive
                {
                    Id = archiveViewModel.Id,
                    Nickname = archiveViewModel.Nickname,
                    OwnerId = archiveViewModel.OwnerId,
                    BirthDate = DateTime.SpecifyKind(archiveViewModel.BirthDate, DateTimeKind.Utc),
                    DeathDate = archiveViewModel.DeathDate != null ? DateTime.SpecifyKind(archiveViewModel.DeathDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                    Gender = archiveViewModel.Gender,
                    Breed = archiveViewModel.Breed,
                    FatherId = archiveViewModel.FatherId,
                    MotherId = archiveViewModel.MotherId,
                    ExhibitionId = archiveViewModel.ExhibitionId
                };

                context.Entry(archive).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(archiveViewModel);
        }




        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var arch = context.Archive.FirstOrDefault(x => x.Id == id);
                if (arch == null)
                {
                    return NotFound();
                }
                context.Archive.Remove(arch);
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
