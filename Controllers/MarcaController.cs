using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SitiosWeb.Model;
using SitiosWeb.servicesclass;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SitiosWeb.Controllers
{
    public class MarcaController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;
        private readonly FaceIDService _faceRecognitionService;  // Añadir el servicio de reconocimiento facial

        public MarcaController(Tiusr22plProyectoContext context)
        {
            _context = context;
            _faceRecognitionService = new FaceIDService();
        }

        [HttpPost]
        public async Task<IActionResult> MarcaNormal(string codigo, string contrasena)
        {
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(contrasena))
            {
                TempData["ErrorMessage"] = "Código y contraseña son requeridos.";
                return RedirectToAction(nameof(Index));  // Asegúrate de que "Index" sea el nombre correcto de la acción
            }

            var colaborador = _context.Usuarios
                .FirstOrDefault(c => c.IdColaborador == codigo && c.Contrasena == contrasena);

            if (colaborador != null)
            {
                try
                {
                    var marca = new Marcas
                    {
                        IdEmpleado = colaborador.IdColaborador,
                        InicioJornada = DateTime.Now
                    };
                    _context.Marcas.Add(marca);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Marca registrada exitosamente.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al registrar la marca: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Usuario o contraseña incorrectos.";
            }

            return RedirectToAction(nameof(Index));  // Asegúrate de que "Index" sea el nombre correcto de la acción
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Marca)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lógica para guardar la marca en la base de datos
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Marca añadida exitosamente.";
                    return RedirectToAction(nameof(Create));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al añadir la marca: " + ex.Message;
                }
            }
            return View("~/Paginas/Marcas/MarcarFaceID.html", Marca);
        }

        [HttpPost]
        public IActionResult ProcessImage([FromBody] JObject data)  // Cambiar a IActionResult y usar [FromBody] para deserializar automáticamente
        {
            var result = _faceRecognitionService.ProcessImage(data);
            return Json(new { message = result });
        }
    }
}


