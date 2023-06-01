using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using kinological_club.Tables;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authorization;
using kinological_club.Models;
using Microsoft.EntityFrameworkCore;

namespace kinological_club.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly DogsDataContext context;

        public AuthController(DogsDataContext context)
        {
            this.context = context;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Login) && !string.IsNullOrEmpty(model.Password))
            {
                // Проверяем логин и пароль в базе данных
                var user = context.Auth.FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    // Аутентификация успешна, выполняйте необходимые действия
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Login) // Добавляем имя пользователя в утверждения
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, "Cookie");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("Cookie", claimsPrincipal);
                    var role = user.Role;

                    // Устанавливаем значение ViewBag.Role на основе значения роли
                    if (role == 1)
                    {
                        /*ViewBag.Role = "admin";
                        ViewBag.Message = "Hello ASP.NET Core";*/
                        TempData["Role"] = "admin";
                    }
                    else if (role == 2)
                    {
                        //ViewBag.Role = "user";
                        TempData["Role"] = "user";
                    }
                    return RedirectToAction("Index", "Dogs");
                    //return View();

                    
                }
            }

            ViewData["ShowErrorMessage"] = true;

            return View();
        }

        public IActionResult Logout(string returnUrl)
        {
            HttpContext.SignOutAsync("Cookie");
            return Redirect("/Home/Index");
        }


    }
}
