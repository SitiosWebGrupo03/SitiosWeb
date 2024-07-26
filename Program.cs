using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddDbContext<Tiusr22plProyectoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Host")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.LogoutPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/AccesoDenegado";
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/Home/Login") && context.Response.StatusCode == 200)
            {
                // Prevent redirect to login page if already authenticated
                context.Response.Redirect("/Home/Index");
            }
            return Task.CompletedTask;
        };
    });
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Adjust as needed
    options.SlidingExpiration = true;
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
