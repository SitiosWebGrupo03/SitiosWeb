using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Security.Claims;

namespace SitiosWeb.Controllers
{
    public class HorasExtrasController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public HorasExtrasController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // Acción para mostrar el formulario de solicitud de horas extras (GET)
        [HttpGet]
        public IActionResult SolicitarHorasExtras()
        {
            // Puedes cargar datos necesarios aquí, si los hay
            return View(); // Asegúrate de tener una vista SolicitarHorasExtras.cshtml
        }

        // Acción para solicitar horas extras (POST)
        [HttpPost]
        public IActionResult SolicitarHorasExtras(DateOnly fecha, decimal horas, int? idTipoActividad)
        {
            var solicitud = new SolicitudHorasExtra
            {
                IdSolicitante = User.FindFirstValue(ClaimTypes.NameIdentifier),
                FechaSolicitud = fecha,
                Horas = horas,
                IdTipoActividad = idTipoActividad,
                Estado = "Pendiente" // Asigna un estado por defecto
            };

            _context.SolicitudHorasExtra.Add(solicitud);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Solicitud de horas extras enviada correctamente.";

            return RedirectToAction("Index", "Home");
        }

        // Acción para aprobar una solicitud de horas extras
        [HttpPost]
        public IActionResult Aprobar(int id)
        {
            var solicitud = _context.SolicitudHorasExtra.FirstOrDefault(x => x.IdSolicitud == id);
            if (solicitud != null)
            {
                solicitud.Estado = "Aprobada"; // Cambia el estado a 'Aprobada'
                solicitud.AprobadaPor = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Solicitud de horas extras aprobada correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
            }

            return RedirectToAction("Index", "Home");
        }

        // Acción para denegar una solicitud de horas extras
        [HttpPost]
        public IActionResult Denegar(int id)
        {
            var solicitud = _context.SolicitudHorasExtra.FirstOrDefault(x => x.IdSolicitud == id);
            if (solicitud != null)
            {
                solicitud.Estado = "Denegada"; // Cambia el estado a 'Denegada'
                solicitud.AprobadaPor = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Solicitud de horas extras denegada correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
            }

            return RedirectToAction("Index", "Home");
        }

        // Acción para mostrar el reporte de solicitudes (para el Supervisor)
        [HttpGet]
        public IActionResult Reporte()
        {
            var solicitudes = _context.SolicitudHorasExtra
                .Include(s => s.IdTipoActividadNavigation) // Incluye los detalles del tipo de actividad
                .ToList();

            return View(solicitudes); // Asegúrate de tener una vista Reporte.cshtml para mostrar las solicitudes
        }
    }
}
