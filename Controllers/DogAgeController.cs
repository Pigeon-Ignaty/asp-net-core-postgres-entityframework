using kinological_club.Models;
using kinological_club.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace kinological_club.Controllers
{
    public class DogAgeController : Controller
    {
        private readonly DogsDataContext context;

        public DogAgeController(DogsDataContext context)
        {
            this.context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var model = new DogAgeViewModel();
            return View(model);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Index(DogAgeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Date1 == DateTime.MinValue)
            {
                model.AgeResult = "Выберите дату 1";
            }
            else
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT get_dog_age(@date1::DATE, @date2::DATE)";

                        var date1Param = command.CreateParameter();
                        date1Param.ParameterName = "@date1";
                        date1Param.Value = model.Date1.Date;
                        command.Parameters.Add(date1Param);

                        var date2Param = command.CreateParameter();
                        date2Param.ParameterName = "@date2";
                        date2Param.Value = model.Date2?.Date ?? DateTime.Today; // Устанавливаем текущую дату, если Date2 пустая
                        command.Parameters.Add(date2Param);

                        var ageResult = command.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(ageResult))
                        {
                            model.AgeResult = ageResult;
                        }
                    }
                }
            }

            return View(model);
        }


    }
}
