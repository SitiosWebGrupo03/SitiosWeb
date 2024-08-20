using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class Indicadores : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public Indicadores(Tiusr22plProyectoContext context)
        {
            _context = context;
        }
        public IActionResult IndexIndicadoresColab()
        {
            var inconsistenciasCount = _context.Inconsistencias
                                        .Where(i => i.IdEmpleado == Request.Cookies["Id"])
                                        .Count();

            var PermisosCount = _context.Permisos
                                        .Where(i => i.IdEmpleado == Request.Cookies["Id"])
                                        .Count();

            var HorasExtrasCount = _context.HorasExtra
                                        .Where(i => i.IdEmpleado == Request.Cookies["Id"])
                                        .Count();

            var MarcasCount = _context.Marcas
                                        .Where(i => i.IdEmpleado == Request.Cookies["Id"])
                                        .Count();

            ViewBag.Inconsistencias = inconsistenciasCount;
            ViewBag.Permisos = PermisosCount;
            ViewBag.HorasExtra = HorasExtrasCount;
            ViewBag.DiasLaburados = MarcasCount;
            return View("~/Views/indicadores/indicadorescolaborador.cshtml");
        }

        public IActionResult IndexIndicadoresJefatura()
        {
            int id = int.Parse(Request.Cookies["IDDepartamento"]);

            var inconsistenciasCount = _context.Inconsistencias
                                        .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
                                        .Count();

            var PermisosCount = _context.Permisos
                                        .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
                                        .Count();

            var HorasExtrasCount = _context.HorasExtra
                                        .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
                                        .Count();

            var MarcasCount = _context.Marcas
                                        .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
                                        .Count();

            ViewBag.Inconsistencias = inconsistenciasCount;
            ViewBag.Permisos = PermisosCount;
            ViewBag.HorasExtra = HorasExtrasCount;
            ViewBag.DiasLaburados = MarcasCount;
            return View("~/Views/indicadores/indicadores.cshtml");
        }

        public IActionResult IndexIndicadoresSupervisor()
        {
            var inconsistenciasCount = _context.Inconsistencias
                                        .Count();

            var PermisosCount = _context.Permisos
                                        .Count();

            var HorasExtrasCount = _context.HorasExtra
                                        .Count();

            var MarcasCount = _context.Marcas
                                        .Count();

            ViewBag.Inconsistencias = inconsistenciasCount;
            ViewBag.Permisos = PermisosCount;
            ViewBag.HorasExtra = HorasExtrasCount;
            ViewBag.DiasLaburados = MarcasCount;
            return View("~/Views/indicadores/indicadores1.cshtml");
        }
    }
}
