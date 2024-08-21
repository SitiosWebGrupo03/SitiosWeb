
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SitiosWeb.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using SitiosWeb.Models;
using Microsoft.Data.SqlClient;
using SitiosWeb.ServicesClass;
using System.Data;

namespace SitiosWeb.Controllers
{
   
    public class MarcaController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;
        private readonly FaceIDService _faceRecognitionService;

        public MarcaController(Tiusr22plProyectoContext context)
        {
            _context = context;
            _faceRecognitionService = new FaceIDService();
        }



        [HttpGet]
        public async Task<IActionResult> VisualizacionMarcas()
        {
            try
            {
                var marcas = await _context.Marcas
                    .Include(m => m.IdEmpleadoNavigation)
                    .ToListAsync();
                return View("~/Views/Marcas/VisualizacionMarcas.cshtml", marcas);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> carcarComboxActividades()
        {
            try
            {
                var actividades = await _context.TipoActividades
                    .Select(p => new
                    {
                        p.IdTipoActividad,
                        p.NomActividad,

                    })
                    .ToListAsync();

                ViewBag.Actividades = actividades;

                return View("~/Views/Marcas/MarcarHex.cshtml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarcaNormal(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                TempData["ErrorMessage"] = "Código es requerido.";
                return RedirectToAction(nameof(MarcaNormal));
            }

            var colaborador = _context.Usuarios
                .FirstOrDefault(c => c.IdColaborador == codigo);

            if (colaborador != null)
            {
                try
                {

                    var idEmpleadoParam = new SqlParameter("@id_empleado", colaborador.IdColaborador);
                    var horaParam = new SqlParameter("@hora", DateTime.Now);


                    var resultadoParam = new SqlParameter("@resultado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC Grupo03.RegistrarMarca @id_empleado, @hora, @resultado OUTPUT",
                        idEmpleadoParam, horaParam, resultadoParam);



                    int resultado = (int)resultadoParam.Value;

                    if (resultado == 1)
                    {
                        TempData["SuccessMessage"] = "Salida tardía registrada.";
                    }
                    else if (resultado == 2)
                    {
                        TempData["SuccessMessage"] = "Entrada temprana registrada.";
                    }
                    else if (resultado == 3)
                    {
                        TempData["SuccessMessage"] = "Marca registrada exitosamente.";
                        IQueryable<Marcas> query = _context.Marcas.Include(m => m.IdEmpleadoNavigation);
                        var marcas = await query.ToListAsync();
                        return View("~/Views/Marcas/MarcaNormalColab.cshtml", marcas);
                    }
                    else if (resultado == 4)
                    {
                        TempData["ErrorMessage"] = "No puedes marcar dentro de los 20 minutos de la última marca.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al registrar la marca: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
            }


            return View("~/Views/Marcas/MarcaNormalColab.cshtml");
        }

        [HttpPost("EditarMarcas")]
        public IActionResult EditarMarcas([FromBody] Marcas marca)
        {
            if (ModelState.IsValid)
            {
                var marcaExistente = _context.Marcas.Find(marca.IdMarca);
                if (marcaExistente != null)
                {
                    marcaExistente.IdEmpleado = marca.IdEmpleado;
                    marcaExistente.InicioJornada = marca.InicioJornada;
                    marcaExistente.FinJornada = marca.FinJornada;
                    _context.SaveChanges();
                }

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public async Task<IActionResult> MarcaHorasExtra(string codigo, string contrasena)
        {
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(contrasena))
            {
                TempData["ErrorMessage"] = "Código y contraseña son requeridos.";
                return RedirectToAction(nameof(MarcaHorasExtra));
            }

            var colaborador = _context.Usuarios
                .FirstOrDefault(c => c.IdColaborador == codigo && c.Contrasena == contrasena);

            if (colaborador != null)
            {
                try
                {
                    var marca = await _context.Database.ExecuteSqlRawAsync(
                        "EXEC RegistrarMarcaHorasExtra @p0, @p1",
                        parameters: new[] { colaborador.IdColaborador, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                    );

                    TempData["SuccessMessage"] = "Marca de horas extra registrada exitosamente.";
                    IQueryable<Marcas> query = _context.Marcas.Include(m => m.IdEmpleadoNavigation);

                    if (!string.IsNullOrEmpty(codigo))
                    {
                        query = query.Where(m => m.IdEmpleado == codigo);
                    }

                    var marcas = await query.ToListAsync();
                    return View("~/Views/Marcas/MarcarHEX.cshtml", marcas);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al registrar la marca de horas extra: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Usuario no encontrado o contraseña incorrecta.";
            }

            return View("~/Views/Marcas/MarcarHEX.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> ProcessImage(string photo)
        {
            try
            {
                if (string.IsNullOrEmpty(photo))
                {
                    TempData["ErrorMessage"] = "No se recibió una imagen.";
                    return View("~/Views/Marcas/MarcarFaceID.cshtml");
                }

                var result = _faceRecognitionService.ProcessImage(photo);

                if (result.StartsWith("No hay coincidencia de rostros.") || result.StartsWith("Error"))
                {
                    TempData["ErrorMessage"] = result;
                    return View("~/Views/Marcas/MarcarFaceID.cshtml");
                }

                // Registro de la marca
                var identificacion = result; // Nombre del archivo, que es el número de cédula

                // Aquí debes buscar el colaborador por identificacion
                var colaborador = await _context.Colaboradores
                    .FirstOrDefaultAsync(c => c.Identificacion == identificacion);

                if (colaborador == null)
                {
                    TempData["ErrorMessage"] = "Colaborador no encontrado.";
                    return View("~/Views/Marcas/MarcarFaceID.cshtml");
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC RegistrarMarca @p0, @p1",
                    new SqlParameter("@p0", colaborador.Identificacion),
                    new SqlParameter("@p1", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                );

                TempData["SuccessMessage"] = $"Marca registrada exitosamente para: {colaborador.Nombre} cédula: {colaborador.Identificacion}";
                return View("~/Views/Marcas/MarcarFaceID.cshtml");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return View("~/Views/Marcas/MarcarFaceID.cshtml");
            }
        }
    }

    }



