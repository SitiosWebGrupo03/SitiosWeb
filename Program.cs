using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Tiusr22plProyectoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Host")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/AccesoDenegado";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JEFATURA", policy => policy.RequireRole("JEFATURA"));
    options.AddPolicy("COLABORADOR", policy => policy.RequireRole("COLABORADOR"));
    options.AddPolicy("SUPERVISOR", policy => policy.RequireRole("SUPERVISOR"));
    // Add more policies as needed
});
var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
