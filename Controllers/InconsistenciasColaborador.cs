using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Net.Mail;
using System.Net;

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

            string config = inconsistenciasPermitidas();

            ViewBag.InconsistenciasPermitidas = config;

            string incon = numeroInconsistencias();
            ViewBag.NumIncon = incon;


            var inconsistencias = await _context.Inconsistencias
             .Include(i => i.IdEmpleadoNavigation)
             .Include(i => i.IdJustificacionNavigation)
             .Include(i => i.IdTipoInconsistenciaNavigation)
             .Where(i => i.IdEmpleadoNavigation.Identificacion == Request.Cookies["Id"]
                    && i.IdJustificacion == null
                    && (i.Mostrar == true || i.Mostrar == null))
             .ToListAsync();
                
            return View(inconsistencias);
        }
        public string inconsistenciasPermitidas()
        {
            var configuracion = _context.Configuraciones
                .FirstOrDefault(c => c.IdConfiguraciones == 2);

            if (configuracion == null || configuracion.NumConfig == null)
            {
                throw new Exception("No se encontró la configuración o NumConfig es nulo.");
            }

            return configuracion.NumConfig.Value.ToString();
        }
        public string numeroInconsistencias()
        {
            if (string.IsNullOrEmpty(Request.Cookies["Id"]))
            {
                throw new ArgumentException("El ID del colaborador es requerido");
            }

            int cantidadInconsistencias = _context.Inconsistencias
                .Count(i => i.IdEmpleadoNavigation.Identificacion == Request.Cookies["Id"]);

            return cantidadInconsistencias.ToString();
        }
        public async Task<IActionResult> Justificacion(string identificacion, string id_puesto, string iddepartamento, int idInconsistencia, int idtipoinconsistencia, string reponetiempo, string horarioid, DateOnly fecha, string observaciones,IFormFile evidencia)
        {
            byte[] evidenciaBytes= null;

            bool reponer = false;
            if (evidencia != null && evidencia.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await evidencia.CopyToAsync(memoryStream);
                    evidenciaBytes = memoryStream.ToArray();
                }
            }
            if (reponetiempo.Equals("si"))
            {
                reponer = true;
            }
            else if (reponetiempo.Equals("no"))
            {
                reponer = false;
            }
            var departament = _context.Departamentos.FirstOrDefault(c => c.NomDepartamento == iddepartamento);
            var justificacion = new JustificacionesInconsistencias
            {
                IdColaborador = identificacion,
                IdPuesto = id_puesto,
                IdDepartamento = departament.IdDepartamento,
                IdTipoInconsistencia = idtipoinconsistencia,
                ReponeTiempo = reponer,
                HorarioId = horarioid,
                FechaInconsistencia = fecha,
                Observaciones = observaciones,
                Evidencias = evidenciaBytes,
                Validacion=null
            };


            _context.JustificacionesInconsistencias.Add(justificacion);
            await _context.SaveChangesAsync();

            var inconsistencia = await _context.Inconsistencias.FindAsync(idInconsistencia);
            if (inconsistencia != null)
            {
                inconsistencia.IdJustificacion = justificacion.IdJustificacion;
                await _context.SaveChangesAsync();
            }

            EnviarCorreo(Request.Cookies["Correo"], "Envio de Justificacion", $"{Request.Cookies["Nombre"]} Usted ha enviado la justificación a su jefatura, se enviará la respuesta por correo");

            return View("~/Views/InconsistenciasColaborador/JustificarInconsistenciasColaborador.cshtml");


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
