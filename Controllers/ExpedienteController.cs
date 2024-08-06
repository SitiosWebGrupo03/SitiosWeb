using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using SitiosWeb.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SitiosWeb.Controllers
{
    public class ExpedienteController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;
        private readonly IWebHostEnvironment _env;

        public ExpedienteController(Tiusr22plProyectoContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> ConsultarHorarioCharge()
        {
            try
            {

                var departamentos = await _context.Departamentos
                    .Where(d => d.Estado)
                    .Select(d => d.NomDepartamento)
                    .ToListAsync();

                var puestos = await _context.Puestos
                    .Where(p => p.Estado)
                    .Select(p => p.NombrePuesto)
                    .ToListAsync();


                ViewBag.Departamentos = departamentos;
                ViewBag.Puestos = puestos;

                return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CargarCombox()
        {
            try
            {

               
                var puestos = await _context.Puestos
                    .Where(p => p.Estado)
                    .Select(p => p.NombrePuesto)
                    .ToListAsync();


                ViewBag.Puestos = puestos;

                return View("~/Views/Shared/HorarioColab.cshtml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ConsultarHorarioColab(string IdPuesto)
        {
            try
            {
                var puestos = await _context.Puestos
                   .Where(p => p.Estado)
                   .Select(p => p.NombrePuesto)
                   .ToListAsync();


                ViewBag.Puestos = puestos;

                var resultados = await _context.HorariosXPuesto
                    .FromSqlRaw(
                    "EXEC ConsultarHorarioPorPuesto @NombrePuesto",
                    new SqlParameter("@NombrePuesto", IdPuesto)
                ).ToListAsync();

                if (resultados.Count == 0)
                {
                    TempData["ErrorMessage"] = "No se encontraron horarios para el puesto y departamento especificados.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Horarios consultados exitosamente.";
                }
               
                // Retornar la vista sin el símbolo ~ y las comillas dobles extra
                return View("~/Views/Shared/HorarioColab.cshtml", resultados);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al consultar los horarios: " + ex.Message;
                return View("HorarioColab");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ConsultarHorario(String Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                TempData["ErrorMessage"] = "La identificación es requerida.";
                return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");
            }

            var colaborador = await _context.Colaboradores.FindAsync(Id);
            if (colaborador == null)
            {
                TempData["ErrorMessage"] = "Colaborador no encontrado.";
                return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");

            }

            try
            {
                var marcas = await _context.HorariosXPuesto
                    .Include(m =>m.IdPuestoNavigation)
                    .ToListAsync();
                return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml", marcas);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error: " + ex.Message });
            }

            //return Json(new { success = true, nombre = colaborador.Nombre, puesto = colaborador.IdPuesto /*, horario = horario*/ });
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
