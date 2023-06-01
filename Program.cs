using kinological_club.Tables;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = "Server=localhost;Database=dogsclub;Port=5432;User Id=postgres;Password="; // Замените на вашу строку подключения к базе данных
builder.Services.AddDbContext<DogsDataContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<DogsDataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DogsclubDb")));
builder.Services.AddAuthentication("Cookie").AddCookie("Cookie", config =>
{
    config.LoginPath = "/Auth/Login";
});
builder.Services.AddAuthorization();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
