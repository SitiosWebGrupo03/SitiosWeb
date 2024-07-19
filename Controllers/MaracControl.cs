using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Sitios_Version_2.servicesclass;

namespace SitiosWeb.Controllers
{
    public class MaracControl(ApplicationDbContext context) : Controller
    {

        private readonly ApplicationDbContext _context = context;
        private readonly FaceIDService _faceRecognitionService = new FaceIDService();  // Añadir el servicio de reconocimiento facial

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Paginas/Marcas/MarcarFaceID.html");
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


