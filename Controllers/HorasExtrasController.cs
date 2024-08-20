using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Net.Mail;
using System.Net;
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
            return View(); 
        }

        [HttpPost]
        public IActionResult SolicitarHorasExtras(string idEmpleado,DateOnly fecha, int horas, int? idTipoActividad)
        {
            var empleado = _context.Colaboradores
                .FirstOrDefault(c => c.Identificacion == idEmpleado);
            var jefatura = _context.Colaboradores
                .FirstOrDefault(c => c.Identificacion == Request.Cookies["Id"]);

            string correo = empleado.Correo;
            string nombre = empleado.Nombre;
            string nomJefe = jefatura.Nombre;
            var solicitud = new SolicitudHorasExtra
            {
                IdSolicitante = Request.Cookies["Id"],
                IdEmpleado = "idEmpleado",
                FechaSolicitud = fecha,
                Horas = horas,
                IdTipoActividad = idTipoActividad,
                Estado = "Pendiente" 
            };

            EnviarCorreo(correo, "Solicitud de horas extras", $"{nombre} se le ha solicitado la realizacion de horas extra por parte de {nomJefe} favor revisar la solicitud");
            _context.SolicitudHorasExtra.Add(solicitud);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Solicitud de horas extras enviada correctamente.";

            return RedirectToAction("~/Views/Paginas/Gestion_Horas_Extras/SolicitarHorasExtras.cshtml");
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
