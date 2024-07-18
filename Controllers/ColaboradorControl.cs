using Microsoft.AspNetCore.Mvc;
using SitiosWeb.Models;
using System.Threading.Tasks;

namespace Sitios_Version_2.Controllers
{
    public class ColaboradorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColaboradorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("AsignarPColaborador");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Colaborador colaborador)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(colaborador);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Colaborador añadido exitosamente.";
                    return RedirectToAction(nameof(Create));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al añadir el colaborador: " + ex.Message;
                }
            }
            return View("AsignarPColaborador", colaborador);
        }
    }
}
