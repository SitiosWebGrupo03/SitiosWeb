using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class VacacionesController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public VacacionesController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        public async  Task<IActionResult> AsignarVacacionesColectivas()
        {
            return View(await _context.BloqueoDias.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> SolicitarVC(string inicio, string fin)
        {
            var vacacionesColectivas = new VacacionesColectivas
            {
                IdSolicitador = Request.Cookies["Id"],
                FechaInicio = DateOnly.Parse(inicio),
                FechaFin = DateOnly.Parse(fin),
                IdDepartamento = _context.Departamentos.Where(u=> u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento

            };
            _context.VacacionesColectivas.Add(vacacionesColectivas);
            await _context.SaveChangesAsync();
            return RedirectToAction("AsignarVacacionesColectivas");
        }
    }
}
