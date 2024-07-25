using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SitiosWeb.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace SitiosWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity !=null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            }
            return View();
        }
        public IActionResult Login()
        {
            return View("~/Paginas/login/login.cshtml");
        }
        [Authorize(Roles = "COLABORADOR")]
        public IActionResult IndexColaborador()
        {
            return View("~/Paginas/Menu/menuColaborador.cshtml");
        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult IndexJefatura()
        {
            var asd = User.Identity.Name;
            return View("~/Paginas/Menu/menuJefatura.cshtml");
        }
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexSupervisor()
        {
            return View("~/Paginas/Menu/menuSupervisor.cshtml");
        }
        public IActionResult AccesoDenegado()
        {
            return View("AccesoDenegado");
        }
        [Route("/Verificar")]
        [HttpGet]
        public IActionResult Verificar()
        {
            bool shouldRedirect = User.Identity.IsAuthenticated;

            // Obtener el rol del usuario
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            string role = roleClaim?.Value ?? string.Empty;

            // Determinar la URL a la que redirigir
            string url;
            switch (role)
            {
                case "COLABORADOR":
                    url = Url.Action("IndexColaborador", "Home");
                    break;
                case "JEFATURA":
                    url = Url.Action("IndexJefatura", "Home");
                    break;
                case "SUPERVISOR":
                    url = Url.Action("IndexSupervisor", "Home");
                    break;
                default:
                    url = Url.Action("Index", "Home");
                    break;
            }

            return Json(new { redirect = shouldRedirect, url = url });
        }
    }
}
