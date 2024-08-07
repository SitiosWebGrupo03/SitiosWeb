using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class SolicitudeRebajoController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public SolicitudeRebajoController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> DescargarEvidencia(int id)
        {
            var justificacion = await _context.JustificacionesInconsistencias
                .FirstOrDefaultAsync(j => j.IdJustificacion == id);

            if (justificacion == null || justificacion.Evidencias == null)
            {
                return NotFound();
            }

            var archivoBytes = justificacion.Evidencias;
            var archivoNombre = $"Evidencia_{justificacion.IdJustificacion}.pdf"; // Ajusta la extensión según el tipo de archivo

            return File(archivoBytes, "application/octet-stream", archivoNombre);
        }

        // GET: SolicitudeRebajo
        public async Task<IActionResult> Index()
        {
            var solicitudes = await _context.SolicitudeRebajo
                .Where(s => s.Mostrar == true || s.Mostrar == null)
                .ToListAsync();
            return View("~/Views/SolicitudeRebajo/Index.cshtml", solicitudes);
        }

        [HttpGet]
        public async Task<IActionResult> GetTiposRebajos()
        {
            var tiposRebajos = await _context.TiposRebajos
                .Select(ti => new { ti.IdTipoRebajo, ti.Cantidad, ti.Descripcion }) 
                .ToListAsync();

            return Json(tiposRebajos);
        }
        public async Task<IActionResult> FormRebajo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitud = await _context.SolicitudeRebajo
                .FirstOrDefaultAsync(s => s.IdSolicitud == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            return View("~/Views/SolicitudeRebajo/evaluacionRebajos.cshtml", solicitud);
        }

        // GET: SolicitudeRebajo/Details/5
        [HttpPost]
        public async Task<IActionResult> solicitarRebajo(int idJustificacion, string observaciones)
        {
            var justificacion = await _context.JustificacionesInconsistencias
                .FirstOrDefaultAsync(j => j.IdJustificacion == idJustificacion);

            if (justificacion == null)
            {
                return NotFound();
            }

            justificacion.Validacion = false;
            _context.Update(justificacion);

            var solicitudRebajo = new SolicitudeRebajo
            {
                IdSolicitante = Request.Cookies["Id"],
                IdInconsistencia = justificacion.IdJustificacion,
                Observaciones = observaciones
            };

            _context.SolicitudeRebajo.Add(solicitudRebajo);
            await _context.SaveChangesAsync();

            var solicitudes = await _context.SolicitudeRebajo
                .Where(s => s.Mostrar == true || s.Mostrar == null)
                .ToListAsync();
            return View("~/Views/SolicitudeRebajo/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> enviarRebajo(int IdSolicitud, string IdSolicitante, int IdInconsistencia, string Observaciones, string IdValidador, int IdTipoRebajo, string FechaRebajo, string IdColaborador, bool Aprobacion)
        {
            DateOnly fechaRebajoParsed;
            if (!DateOnly.TryParse(FechaRebajo, out fechaRebajoParsed))
            {
                ModelState.AddModelError("FechaRebajo", "La fecha no está en el formato correcto.");
                return View();
            }

            var solicitudRebajo = await _context.SolicitudeRebajo.FindAsync(IdSolicitud);
            if (solicitudRebajo == null)
            {
                return NotFound();
            }

            var rebajo = new Rebajos
            {
                IdColaborador = IdColaborador,
                IdValidador = IdValidador,
                FechaRebajo = fechaRebajoParsed,
                Inconsistencia = IdInconsistencia,
                IdTipoRebajo = IdTipoRebajo,
                Aprobacion = Aprobacion,
            };

            _context.Rebajos.Add(rebajo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> rechazarRebajo(int idJustificacion)
        {
            var justificacion = await _context.JustificacionesInconsistencias
                .FirstOrDefaultAsync(j => j.IdJustificacion == idJustificacion);

            if (justificacion == null)
            {
                return NotFound();
            }

            justificacion.Validacion = true;
            _context.Update(justificacion);

            await _context.SaveChangesAsync();

            return View();
        }

        private bool SolicitudeRebajoExists(int id)
        {
            return _context.SolicitudeRebajo.Any(e => e.IdSolicitud == id);
        }
    }
}
