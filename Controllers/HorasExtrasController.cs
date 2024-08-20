using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

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
            return View("~/Views/Paginas/Gestion_Horas_Extras/SolicitarHorasExtras.cshtml");
        }

        [HttpPost]
        public IActionResult SolicitarHorasExtras(string idEmpleado, DateOnly fecha, decimal horas, int? idTipoActividad)
        {
            var empleado = _context.Colaboradores
                .FirstOrDefault(c => c.Identificacion == idEmpleado);

            if (empleado == null)
            {
                TempData["ErrorMessage"] = "El empleado no existe.";
                return RedirectToAction("SolicitarHorasExtras");
            }

            var jefatura = _context.Colaboradores
                .FirstOrDefault(c => c.Identificacion == Request.Cookies["Id"]);

            if (jefatura == null)
            {
                TempData["ErrorMessage"] = "No se pudo identificar al jefe.";
                return RedirectToAction("SolicitarHorasExtras");
            }

            var solicitud = new SolicitudHorasExtra
            {
                IdSolicitante = Request.Cookies["Id"],
                IdEmpleado = idEmpleado,
                FechaSolicitud = fecha,
                Horas = horas,
                IdTipoActividad = idTipoActividad,
                Estado = "Pendiente"
            };

            if (idTipoActividad.HasValue)
            {
                var tipoActividad = _context.TipoActividades
                    .FirstOrDefault(t => t.IdTipoActividad == idTipoActividad.Value);

                if (tipoActividad == null)
                {
                    TempData["ErrorMessage"] = "Tipo de actividad no válido.";
                    return RedirectToAction("SolicitarHorasExtras");
                }
            }

            EnviarCorreo(empleado.Correo, "Solicitud de horas extras", $"{empleado.Nombre} ha solicitado horas extras por parte de {jefatura.Nombre}. Favor revisar la solicitud.");
            _context.SolicitudHorasExtra.Add(solicitud);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Solicitud de horas extras enviada correctamente.";
            return RedirectToAction("SolicitarHorasExtras");
        }

        private void EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("calderonmora6@gmail.com", "qsre xvxi yyvt flyw"),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("calderonmora6@gmail.com"),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(destinatario);

                smtpClient.Send(mailMessage);
                Console.WriteLine("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Aprobar(int id)
        {
            var solicitud = _context.SolicitudHorasExtra
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdSolicitanteNavigation)
                .FirstOrDefault(x => x.IdSolicitud == id);

            if (solicitud != null)
            {
                solicitud.Estado = "Aprobada";
                solicitud.AprobadaPor = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Solicitud de horas extras aprobada correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
            }

            return RedirectToAction("Reporte");
        }

        [HttpPost]
        public IActionResult Denegar(int id)
        {
            var solicitud = _context.SolicitudHorasExtra
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdSolicitanteNavigation)
                .FirstOrDefault(x => x.IdSolicitud == id);

            if (solicitud != null)
            {
                solicitud.Estado = "Denegada";
                solicitud.AprobadaPor = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Solicitud de horas extras denegada correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
            }

            return RedirectToAction("Reporte");
        }

        [HttpGet]
        public IActionResult Reporte()
        {
            var solicitudes = _context.SolicitudHorasExtra
                .Include(s => s.IdTipoActividadNavigation)
                .Include(s => s.IdEmpleadoNavigation)  // Incluye información del empleado
                .Include(s => s.IdSolicitanteNavigation) // Incluye información del solicitante
                .ToList();

            return View(solicitudes);
        }
    }
}
