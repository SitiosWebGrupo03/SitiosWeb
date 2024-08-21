﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using SitiosWeb.Models;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace SitiosWeb.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<IActionResult> CargarComboxAP()
        {
            try
            {


                var puestos = await _context.Puestos
                    .Where(p => p.Estado)
                    .Select(p => p.NombrePuesto)
                    .ToListAsync();


                ViewBag.Puestos = puestos;

                return View("~/Views/ExpedienteEmpleado/AsignarPColab.cshtml");
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
        public async Task<IActionResult> AsignarPuestoColab(string identificacion, string puesto)
        {
            try
            {
                var puestos = await _context.Puestos
                    .Where(p => p.Estado)
                    .Select(p => p.NombrePuesto)
                    .ToListAsync();

                ViewBag.Puestos = puestos;

                // Ejecutar el procedimiento almacenado sin devolver una lista
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC ActualizarPuestoColaborador @identificacion, @nombre_puesto",
                    new SqlParameter("@identificacion", identificacion),
                    new SqlParameter("@nombre_puesto", puesto)
                );

                TempData["SuccessMessage"] = "Puesto asignado exitosamente.";

                // Retornar la vista
                return View("~/Views/ExpedienteEmpleado/AsignarPColab.cshtml");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al asignar el puesto: " + ex.Message;
                return View("~/Views/ExpedienteEmpleado/AsignarPColab.cshtml");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ConsultarHorarioJef(string identificacion)
        {
            try
            {
                var horario = await _context.HorariosXPuesto
                    .FromSqlRaw("EXEC ConsultaHcedula @cedula", new SqlParameter("@cedula", identificacion))
                    .ToListAsync();
                bool tieneError = horario == null || horario.Count == 0||
     horario.Any(h =>
         h.IdPuesto == "N/A"||
         h.Lunes == "N/A"||
         h.Martes == "N/A"||
         h.Miercoles == "N/A"||
         h.Jueves == "N/A"||
         h.Viernes == "N/A"||
         h.Sabado == "N/A" ||
         (h.Estado.HasValue && h.Estado.Value == false));

                if (tieneError)
                {
                    TempData["ErrorMessage"] = "No existe colaborador con estos datos.";
                    return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");
                }

                ViewBag.Horarios = horario;

                TempData["SuccessMessage"] = "Horario consultado exitosamente.";

                // Retornar la vista
                return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al consultar el horario: " + ex.Message;
                return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");
            }
        }


        public async Task<IActionResult> CargarComboxEditaP()
        {
            try
            {
                var puestos = await _context.Puestos
                    .Select(p => new
                    {
                        p.IdPuesto,
                        p.NombrePuesto,
                        
                    })
                    .ToListAsync();

                ViewBag.Puestos = puestos;

                return View("~/Views/ExpedienteEmpleado/Modificar.cshtml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error: " + ex.Message });
            }
        }
        [HttpPost]
           public IActionResult EditarPuesto(string idPuesto, string identificacion)
        {
            if (ModelState.IsValid)
            {
                var usuario = _context.Colaboradores.FirstOrDefault(u => u.Identificacion == identificacion);
                var puesto = _context.Puestos.FirstOrDefault(p => p.IdPuesto == idPuesto);

                if (usuario == null)
                {
                    // Manejar el caso donde el usuario no existe
                    ModelState.AddModelError(string.Empty, "El usuario con esa identificación no existe.");
                    ViewBag.Puestos = _context.Puestos.ToList(); // Asegurarse de recargar los puestos
                    return View(); // Retorna la vista con el mensaje de error
                }

                if (puesto == null)
                {
                    // Manejar el caso donde el puesto no existe
                    ModelState.AddModelError(string.Empty, "El puesto con ese ID no existe.");
                    ViewBag.Puestos = _context.Puestos.ToList(); // Asegurarse de recargar los puestos
                    return View(); // Retorna la vista con el mensaje de error
                }

                // Actualizar el puesto del usuario
                usuario.IdPuesto = idPuesto;
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }




        [HttpPost]
        public async Task<IActionResult> CrearUsuario(string codigoUsuario, int TipoUsuario, string Contrasena, string IdColaborador, int Estado)
        {
            try
            {
                // Definir el parámetro de salida para el resultado
                //var resultParam = new SqlParameter("@result", SqlDbType.NVarChar, 100)
                //{
                //    Direction = ParameterDirection.Output
                //};

                // Ejecutar el procedimiento almacenado
               var result= await _context.Database.ExecuteSqlRawAsync(
                    "EXEC InsertUsuario @cod_usuario, @idTipoUsuario, @contrasena, @idColaborador, @estado ",
                    new SqlParameter("@cod_usuario", codigoUsuario),
                    new SqlParameter("@idTipoUsuario", TipoUsuario),
                    new SqlParameter("@contrasena", Contrasena),
                    new SqlParameter("@idColaborador", IdColaborador),
                    new SqlParameter("@estado", Estado)
                    
                );

                // Obtener el resultado del parámetro de salida
                //string result = (string)resultParam.Value;
                if (result == 1)
                {

                    TempData["SuccessMessage"] = "Se creo el ususario correctamente";



                }else if (result == -1) {

                    TempData["SuccessMessage"] = "No existe el colaborador"; 

                }
                // Manejar el resultado según sea necesario
               

                // Retornar la vista
                return View("~/Views/Shared/CreacionUsuario.cshtml");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear usuario: " + ex.Message;
                return View("~/Views/Shared/CreacionUsuario.cshtml");
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
        public async Task<IActionResult> AgregarColaborador([FromForm] Colaboradores colaboradorDto, [FromForm] string photoBase64)
        {
            if (colaboradorDto == null || string.IsNullOrEmpty(photoBase64))
            {
                return Json(new { success = false, message = "Todos los campos son requeridos." });
            }

            var colaborador = new Colaboradores
            {
                Identificacion = colaboradorDto.Identificacion,
                Nombre = colaboradorDto.Nombre,
                Apellidos = colaboradorDto.Apellidos,
                FechaNaciento = colaboradorDto.FechaNaciento,
                FechaContratacion = colaboradorDto.FechaContratacion,
                FechaFinContrato = colaboradorDto.FechaFinContrato,
                Correo = colaboradorDto.Correo,
                Telefono = colaboradorDto.Telefono
            };

            try
            {
                _context.Colaboradores.Add(colaborador);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(photoBase64))
                {
                    string imageType = "jpeg";
                    if (photoBase64.StartsWith($"data:image/{imageType};base64,"))
                    {
                        var base64Data = photoBase64.Replace($"data:image/{imageType};base64,", "");
                        var imageBytes = Convert.FromBase64String(base64Data);

                        var imagePath = Path.Combine(_env.WebRootPath, "imageFaceID", $"{colaboradorDto.Identificacion}.jpg");
                        await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                    }
                    else
                    {
                        return Json(new { success = false, message = "El formato de la imagen no es vÃ¡lido." });
                    }
                }

                return Json(new { success = true, message = "Colaborador agregado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al agregar el colaborador: " + ex.Message });
            }
        }


        public IActionResult AgregarColaborador()
        {
            return View("~/Views/ExpedienteEmpleado/AgregarC.cshtml");
        }

        public class ImagenDto
        {
            public string ImagenBase64 { get; set; }
        }




    }

   


}
