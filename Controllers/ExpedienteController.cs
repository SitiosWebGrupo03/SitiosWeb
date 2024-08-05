using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SitiosWeb.Model;
using SitiosWeb.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SitiosWeb.Controllers
{
    public class ExpedienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ExpedienteController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        [HttpPost("ConsultarHorario")]
        public async Task<IActionResult> ConsultarHorario(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                TempData["ErrorMessage"] = "La identificación es requerida.";
                return RedirectToAction(nameof(ConsultarHorario));
            }

            var colaborador = await _context.Colaboradores.FindAsync(identificacion);
            if (colaborador == null)
            {
                TempData["ErrorMessage"] = "Colaborador no encontrado.";
                return RedirectToAction(nameof(ConsultarHorario));
            }

            // Aquí deberías tener lógica para obtener el horario del colaborador
            // var horario = ObtenerHorario(colaborador);

            return Json(new { success = true, nombre = colaborador.Nombre, puesto = colaborador.IdPuesto /*, horario = horario*/ });
        }




        [HttpPost("Asignar")]
        public async Task<IActionResult> AgregarColaborador(string identificacion, string nombre,
            string apellidos,
            DateOnly fechaNacimiento,
            DateOnly fechaContratacion,
            DateOnly fechaFinContrato,
            string correo,
            int telefono,
            [FromBody] ImagenDto imagenDto)
        {
            if (string.IsNullOrWhiteSpace(identificacion) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(correo) || telefono == 0)
            {
                TempData["ErrorMessage"] = "Todos los campos son requeridos.";
                return RedirectToAction(nameof(AgregarColaborador));
            }

            var colaborador = new Colaboradores
            {
                Identificacion = identificacion,
                Nombre = nombre,
                Apellidos = apellidos,
                FechaNaciento= fechaNacimiento,
                FechaContratacion = fechaContratacion,
                FechaFinContrato = fechaFinContrato,
                Correo = correo,
                Telefono = telefono
            };

            try
            {
                _context.Colaboradores.Add(colaborador);
                await _context.SaveChangesAsync();

                // Guardar la imagen
                if (!string.IsNullOrEmpty(imagenDto.ImagenBase64))
                {
                    var imagePath = Path.Combine(_env.WebRootPath, "imageFaceID", $"{identificacion}.png");
                    var imageBytes = Convert.FromBase64String(imagenDto.ImagenBase64.Replace("data:image/png;base64,", ""));
                    await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                }

                TempData["SuccessMessage"] = "Colaborador agregado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al agregar el colaborador: " + ex.Message;
            }

            return View("~/Views/ExpedienteEmpleado/AgregarColaborador.cshtml");
        }
    }

    public class ImagenDto
    {
        public string ImagenBase64 { get; set; }
    }


}
