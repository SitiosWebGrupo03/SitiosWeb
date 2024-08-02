using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    
    public class InconsistenciasColaborador : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public InconsistenciasColaborador(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(Request.Cookies["Id"]))
            {
                return BadRequest("El ID del colaborador es requerido");
            }

            var colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.Identificacion == Request.Cookies["Id"]);

            if (colaborador == null)
            {
                return NotFound("Colaborador no encontrado");
            }

            var inconsistencias = await _context.Inconsistencias
                .Include(i => i.IdEmpleadoNavigation)
                .Include(i => i.IdJustificacionNavigation)
                .Include(i => i.IdTipoInconsistenciaNavigation)
                .Where(i => i.IdEmpleadoNavigation.Identificacion == Request.Cookies["Id"])
                .ToListAsync();

            return View(inconsistencias);
        }

        public IActionResult Identificacion()
        {
            var idCookie = Request.Cookies["Id"];

            ViewBag.IdCookie = idCookie;

            return View();
        }

        public IActionResult Puesto()
        {
            var idCookie = Request.Cookies["Puesto"];

            ViewBag.IdCookie = idCookie;

            return View();
        }

        public IActionResult Departamento()
        {
            var idCookie = Request.Cookies["Departamento"];

            ViewBag.IdCookie = idCookie;

            return View();
        }
        public async Task<IActionResult> Justificar()
        {
            //if (string.IsNullOrEmpty(Request.Cookies["Id"]))
            //{
            //    return BadRequest("El ID del colaborador es requerido");
            //}

            //var colaborador = await _context.Colaboradores
            //    .FirstOrDefaultAsync(c => c.Identificacion == Request.Cookies["Id"]);

            //if (colaborador == null)
            //{
            //    return NotFound("Colaborador no encontrado");
            //}

            //var inconsistencias = await _context.Inconsistencias
            //    .Include(i => i.IdEmpleadoNavigation)
            //    .Include(i => i.IdJustificacionNavigation)
            //    .Include(i => i.IdTipoInconsistenciaNavigation)
            //    .Where(i => i.IdEmpleadoNavigation.Identificacion == Request.Cookies["Id"])
            //    .ToListAsync();

            //return View(inconsistencias);
            return View("~/Views/InconsistenciasColaborador/JustificarInconsistenciasColaborador.cshtml");
        }
    }
}
