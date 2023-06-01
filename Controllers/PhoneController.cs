using kinological_club.Models;
using kinological_club.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

public class PhoneController : Controller
{
    private readonly DogsDataContext _context;

    public PhoneController(DogsDataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new PhoneViewModel();
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Index(PhoneViewModel model)
    {
        using (var connection = _context.Database.GetDbConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT format_phone_number(@phoneNumber)";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@phoneNumber";
                parameter.Value = model.PhoneNumber;

                // Проверка на пустую строку
                if (string.IsNullOrEmpty(model.PhoneNumber))
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
                        model.FormattedPhoneNumber = formattedPhoneNumber;
                    }
                }
            }
        }

        return View(model);
    }
}
