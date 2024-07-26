using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SitiosWeb.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Linq;

namespace SitiosWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Tiusr22plProyectoContext _context;

        public HomeController(ILogger<HomeController> logger, Tiusr22plProyectoContext context)
        {
            _logger = logger;
            _context = context;
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
            var Usuario =_context.Usuarios.TagWith("Usuario").ToList();
            TempData["Datos"] = Usuario;
            TempData["Error"] = "Moncho rico.";
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
            return View("~/Paginas/Menu/menuJefatura.cshtml");
        }
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexSupervisor()
        {
            return View("~/Paginas/Menu/menuSup.cshtml");
        }
        public IActionResult AccesoDenegado()
        {
            return View("AccesoDenegado");
        }
        [Authorize(Roles = "COLABORADOR")]
        public IActionResult solicitudRepo() {
            return View("~/Paginas/reposiciones/SolicitudReposicion.cshtml");
        }
        [Authorize(Roles = "COLABORADOR")]
        public IActionResult indicadoresColab()
        {
            return View("~/Paginas/indicadores/indicadorescolaborador.cshtml");
        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult VistaInconsistencias()
        {
            var inconsistencias = _context.Inconsistencias
                                          .Include(i => i.IdEmpleadoNavigation)
                                          .Include(i => i.IdJustificacionNavigation)
                                          .Include(i => i.IdTipoInconsistenciaNavigation)
                                          .ToList();

            return View("~/Views/Inconsistencias/Index.cshtml", inconsistencias);
        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult SelectRepos() {
            var reposiciones = _context.Reposiciones
                                          .Include(r => r.IdcolaboradorNavigation)
                                          .ToList();
            return View("~/Views/seleccionarRepo.cshtml", reposiciones);
        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult AprobarRepo(string id)
        {
            var reposicion = _context.FechasReposicion
                             .Include(r => r.IdReposicionNavigation) // Ensure this navigation property is correctly set
                             .Include(r => r.IdReposicionNavigation.IdcolaboradorNavigation) // Ensure this navigation property is correctly set
                             .Where(r => r.IdReposicionNavigation.Idcolaborador == id) // Filter the results
                             .ToList(); // Execute the query and get the results

            return View("~/Paginas/reposiciones/aprobacionRepo.cshtml", reposicion);
        }
    }
}
