using kinological_club.Models;
using kinological_club.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

public class ShortenNameController : Controller
{
    private readonly DogsDataContext _context;

    public ShortenNameController(DogsDataContext context)
    {
        _context = context;
    }
    [Authorize]

    public IActionResult Index()
    {
        return View(new ShortenNameViewModel());
    }

    [HttpPost]
    [Authorize]
    public IActionResult Index(ShortenNameViewModel model)
    {
        var fullName = model.FullName;

        using (var connection = _context.Database.GetDbConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT shorten_name(@fullName)";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@fullName";
                parameter.Value = fullName;

                // Проверка на пустую строку
                if (string.IsNullOrEmpty(fullName))
                {
                    // Вместо пустой строки установите значение NULL или другое допустимое значение
                    parameter.Value = DBNull.Value;
                }

                command.Parameters.Add(parameter);

                // Проверка наличия значения параметра
                if (parameter.Value != DBNull.Value)
                {
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        var shortenedName = result.ToString();

                        var resultModel = new ShortenNameViewModel
                        {
                            FullName = fullName,
                            ShortenedName = shortenedName
                        };

                        return View(resultModel);
                    }
                }
            }
        }

        return View(model);
    }


    [HttpPost]
    [Authorize]
    public IActionResult FormatPhoneNumber(string phoneNumber)
    {
        using (var connection = _context.Database.GetDbConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT format_phone_number(@phoneNumber)";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@phoneNumber";
                parameter.Value = phoneNumber;

                // Проверка на пустую строку
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    // Вместо пустой строки установите значение NULL или другое допустимое значение
                    parameter.Value = DBNull.Value;
                }

                command.Parameters.Add(parameter);

                // Проверка наличия значения параметра
                if (parameter.Value != DBNull.Value)
                {
                    var formattedPhoneNumber = command.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(formattedPhoneNumber))
                    {
                        return Content(formattedPhoneNumber);
                    }
                }
            }
        }

        return Content("");
    }

}
