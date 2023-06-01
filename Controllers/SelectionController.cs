using kinological_club.Models;
using kinological_club.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

public class SelectionController : Controller
{
    private readonly DogsDataContext _context;

    public SelectionController(DogsDataContext context)
    {
        _context = context;
    }
    [Authorize]

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    [Authorize]

    [HttpPost]
    public IActionResult Index(string breed)
    {
        using (var connection = _context.Database.GetDbConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM select_dogs_for_exhibition_f(@breed)";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@breed";
                parameter.Value = breed;
                ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text; // Установка типа параметра
                command.Parameters.Add(parameter);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var dogId = reader.GetInt32(0);
                            var ageString = reader.GetFieldValue<TimeSpan>(1).ToString();
                            var awardsCount = reader.GetInt64(2);
                            var breedString = reader.GetString(3);

                            TimeSpan age;
                            if (TimeSpan.TryParse(ageString, out var parsedAge))
                            {
                                age = parsedAge;
                            }
                            else
                            {
                                // Обработка ошибки парсинга
                                // Например, установка значения по умолчанию или выброс исключения
                                age = TimeSpan.Zero;
                            }

                            var resultModel = new SelectionViewModel
                            {
                                DogId = dogId,
                                Age = (int)age.TotalDays,
                                AwardsCount = (int)awardsCount,
                                Breed = breedString
                            };

                            return View(resultModel);
                        }
                    }
                }
            }
        }

        // Создание пустого объекта SelectionViewModel, если функция не вернула ни одной строки
        var emptyModel = new SelectionViewModel();
        return View(emptyModel);
    }














}