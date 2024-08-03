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

        public async Task<IActionResult> Justificacion(string identificacion, string id_puesto, int iddepartamento, int idInconsistencia, int idtipoinconsistencia, string reponetiempo, string horarioid, DateOnly fecha, string observaciones,byte[] evidencia)
        {

            bool reponer = false;

            if (reponetiempo.Equals("si"))
            {
                reponer = true;
            }
            else if (reponetiempo.Equals("no"))
            {
                reponer = false;
            }
            var justificacion = new JustificacionesInconsistencias
            {
                IdColaborador = identificacion,
                IdPuesto = id_puesto,
                IdDepartamento = iddepartamento,
                IdTipoInconsistencia = idtipoinconsistencia,
                ReponeTiempo = reponer,
                HorarioId = horarioid,
                FechaInconsistencia = fecha,
                Observaciones = observaciones,
                Evidencias = evidencia
            };


            _context.JustificacionesInconsistencias.Add(justificacion);
            await _context.SaveChangesAsync();

            var inconsistencia = await _context.Inconsistencias.FindAsync(idInconsistencia);
            if (inconsistencia != null)
            {
                inconsistencia.IdJustificacion = justificacion.IdJustificacion;
                await _context.SaveChangesAsync();
            }

            return View("~/Views/InconsistenciasColaborador/JustificarInconsistenciasColaborador.cshtml");


        }


        public async Task<IActionResult> Justificar(int id)
        {
            var inconsistencia = await _context.Inconsistencias.FindAsync(id);

            if (inconsistencia != null && inconsistencia.IdJustificacion != null)
            {
                return Content("Esta inconsistencia ya tiene una justificación asociada.");
            }

            ViewBag.IdInconsistencia = id;
            return View("~/Views/InconsistenciasColaborador/JustificarInconsistenciasColaborador.cshtml");
        }
        public async Task<IActionResult> GetTiposInconsistencias()
        {
            var tiposInconsistencias = await _context.TiposInconsistencias
                .Select(ti => new { ti.IdTipoInconsistencia, ti.Descripcion })
                .ToListAsync();

            return Json(tiposInconsistencias);
        }
    }
}
