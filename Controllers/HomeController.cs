using kinological_club.Models;
using kinological_club.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace kinological_club.Controllers
{
    public class HomeController : Controller
    {
        private readonly DogsDataContext context;
        private readonly ILogger<HomeController> logger;

        public HomeController(DogsDataContext context, ILogger<HomeController> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var dogs = context.Dogs.Include(d => d.Owner).ToList();

            var ownersWithMultipleDogs = dogs
                .GroupBy(dog => dog.OwnerId)
                .Where(group => group.Count() >= 2)
                .Select(group => new OwnersViewModel
                {
                    OwnerId = group.Key,
                    LastName = group.First().Owner.LastName,
                    FirstName = group.First().Owner.FirstName,
                    MiddleName = group.First().Owner.MiddleName,
                    Address = group.First().Owner.Address,
                    Phone = group.First().Owner.Phone,
                    DogCount = group.Count()
                })
                .OrderByDescending(owner => owner.DogCount)
                .ToList();

            var breedStatistics = dogs
    .GroupBy(dog => dog.Breed)
    .Select(group => new BreedStatisticsViewModel
    {
        Breed = group.Key,
        DogCount = group.Count(),
        EarliestBirthDate = group.Min(dog => dog.BirthDate),
        LatestBirthDate = group.Max(dog => dog.BirthDate)
    })
    .OrderByDescending(stat => stat.DogCount)
    .ToList();


            var dogsViewModel = new DogsViewModel
            {
                Owners = ownersWithMultipleDogs,
                Dogs = dogs.Select(dog => new DogsViewModel
                {
                    Id = dog.Id,
                    Nickname = dog.Nickname,
                    OwnerId = dog.OwnerId,
                    BirthDate = dog.BirthDate,
                    DeathDate = dog.DeathDate,
                    Gender = dog.Gender,
                    Breed = dog.Breed,
                    FatherId = dog.FatherId,
                    MotherId = dog.MotherId,
                    ExhibitionId = dog.ExhibitionId
                }).ToList(),
                DogCount = dogs.Count,
                BreedStatistics = breedStatistics
                
            };

            return View(dogsViewModel);
        }



        public IActionResult Contacts()
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