using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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

            int? inconsistencia = solicitud.IdInconsistencia;

            var incon = await _context.Inconsistencias
                .FirstOrDefaultAsync(s => s.IdInconsistencia == inconsistencia);

            int? tipo = incon.IdTipoInconsistencia;

            var tipoIncon = await _context.TiposInconsistencias
                .FirstOrDefaultAsync(s => s.IdTipoInconsistencia == tipo);

            string nomTipo = tipoIncon.Descripcion;

            string identificacion = incon.IdEmpleado.ToString();

            var empleado = await _context.Colaboradores
                .FirstOrDefaultAsync(s => s.Identificacion == identificacion);

            string depa = empleado.IdPuesto.ToString();

            var puestos = await _context.Puestos
                .FirstOrDefaultAsync(s => s.IdPuesto == depa);

            int? idDepa = puestos.IdDepartamento;

            var departamentos = await _context.Departamentos
                .FirstOrDefaultAsync(s => s.IdDepartamento == idDepa);

            string nomDepa = departamentos.NomDepartamento.ToString();

            string nombre = empleado.Nombre.ToString();

            ViewBag.Identificacion = identificacion;
            ViewBag.Nombre = nombre;
            ViewBag.Departamento = nomDepa;
            ViewBag.Inconsistencia = nomTipo;

            if (solicitud == null)
            {
                return NotFound();
            }

            return View("~/Views/SolicitudeRebajo/evaluacionRebajos.cshtml", solicitud);
        }

        [HttpPost]
        public async Task<IActionResult> solicitarRebajo(int idJustificacion, string observaciones)
        {
            var justificacion = await _context.JustificacionesInconsistencias
                .FirstOrDefaultAsync(j => j.IdJustificacion == idJustificacion);

            string idColaborador = justificacion?.IdColaborador.ToString();

            var colab = await _context.Colaboradores
                .FirstOrDefaultAsync(j => j.Identificacion == idColaborador);

            string correo = colab?.Correo.ToString();
            string nombre = colab?.Nombre.ToString();


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

            var inconsistencia = await _context.Inconsistencias
             .FirstOrDefaultAsync(i => i.IdJustificacion == idJustificacion);

            if (inconsistencia != null)
            {
                _context.Entry(inconsistencia).State = EntityState.Modified;
                inconsistencia.Mostrar = false;
                _context.Inconsistencias.Update(inconsistencia);
                await _context.SaveChangesAsync();
            }

            var solicitudes = await _context.SolicitudeRebajo
                .Where(s => s.Mostrar == true || s.Mostrar == null)
                .ToListAsync();

            EnviarCorreo(correo, "Justificacion de tu inconsistencia", $"Querido {nombre} su justificacion ha sido rechazada, por lo tanto se ha enviado una solicitud para que le sea aplicado el rebajo correspondiente");

            return View("~/Views/SolicitudeRebajo/Index.cshtml");
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

            int? idInconsistencia = solicitudRebajo.IdInconsistencia;
            var inconsistencia = await _context.Inconsistencias
                .FirstOrDefaultAsync(s => s.IdInconsistencia == idInconsistencia);

            string idColab = inconsistencia.IdEmpleado;
            var colaborador = await _context.Colaboradores
               .FirstOrDefaultAsync(s => s.Identificacion == idColab);

            string correo = colaborador.Correo;
            string nombre = colaborador.Nombre;

            if (Aprobacion == false) 
            {
                solicitudRebajo.Mostrar = false;
                await _context.SaveChangesAsync();
                EnviarCorreo(correo, "Solicitud de Rebajo", $"Estimado {nombre} la solicitud de rebajo ha sido rechazada, por lo tanto no sera aplicado a su salario");
                var soli = await _context.SolicitudeRebajo
                           .Where(s => s.Mostrar == true || s.Mostrar == null)
                           .ToListAsync();
                return View("~/Views/SolicitudeRebajo/Index.cshtml", soli);
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

            var tipoRebajo = await _context.TiposRebajos
               .FirstOrDefaultAsync(s => s.IdTipoRebajo == IdTipoRebajo);
            int? cantidad = tipoRebajo.Cantidad;
            string desc = tipoRebajo.Descripcion;

            solicitudRebajo.Mostrar = false;
            await _context.SaveChangesAsync();
            _context.Rebajos.Add(rebajo);
            await _context.SaveChangesAsync();
            EnviarCorreo(correo, "Solicitud de Rebajo", $"Estimado {nombre} la solicitud de rebajo ha sido aceptada, por lo tanto se le aplicara un rebajo por {desc} con una cantidad de ₡{cantidad}  en su siguiente fecha de pago");

            var solicitudes = await _context.SolicitudeRebajo
                           .Where(s => s.Mostrar == true || s.Mostrar == null)
                           .ToListAsync();
            return View("~/Views/SolicitudeRebajo/Index.cshtml", solicitudes);
        }

        [HttpPost]
        public async Task<IActionResult> rechazarRebajo(int idJustificacion)
        {
            var justificacion = await _context.JustificacionesInconsistencias
                .FirstOrDefaultAsync(j => j.IdJustificacion == idJustificacion);

            string idColaborador = justificacion?.IdColaborador.ToString();

            var colab = await _context.Colaboradores
                .FirstOrDefaultAsync(j => j.Identificacion == idColaborador);

            string correo = colab?.Correo.ToString();
            string nombre = colab?.Nombre.ToString();

            if (justificacion == null)
            {
                return NotFound();
            }

            justificacion.Validacion = true;
            _context.Update(justificacion);

            await _context.SaveChangesAsync();

            var inconsistencia = await _context.Inconsistencias
             .FirstOrDefaultAsync(i => i.IdJustificacion == idJustificacion);

            if (inconsistencia != null)
            {
                _context.Entry(inconsistencia).State = EntityState.Modified;
                inconsistencia.Mostrar = false;
                _context.Inconsistencias.Update(inconsistencia);
                await _context.SaveChangesAsync();
            }

            EnviarCorreo(correo, "Justificacion de tu inconsistencia", $"Querido {nombre} su justificacion ha sido aceptada, por lo tanto no se enviara una solicitud de rebajo a la jefatura");
            return View("~/Views/SolicitudeRebajo/Index.cshtml");
        }

        private bool SolicitudeRebajoExists(int id)
        {
            return _context.SolicitudeRebajo.Any(e => e.IdSolicitud == id);
        }
    }
}
