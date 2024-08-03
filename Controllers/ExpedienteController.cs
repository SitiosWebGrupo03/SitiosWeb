using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SitiosWeb.Model;
using System;
using System.Threading.Tasks;

namespace SitiosWeb.Controllers
{
    public class ColaboradorController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public ColaboradorController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        [HttpPost("Asignar")]
        public async Task<IActionResult> AgregarColaborador([FromBody]Colaboradores colaborador)
        {
            if (colaborador==null)
            {
                TempData["ErrorMessage"] = "Todos los campos son requeridos.";
                return RedirectToAction(nameof(AgregarColaborador));
            }

            try
            {
                _context.Colaboradores.Add(colaborador);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Colaborador agregado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al agregar el colaborador: " + ex.Message;
            }

            return View("~/Views/ExpedienteEmpleado/AgregarColaborador.cshtml");
        }

        //[HttpGet]
        //public IActionResult AgregarColaborador()
        //{
        //    return View();
        //}
    }
}
