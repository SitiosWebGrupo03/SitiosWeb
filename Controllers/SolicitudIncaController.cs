using Microsoft.AspNetCore.Mvc;
using SitiosWeb.Model;
using SitiosWeb.ServicesClass;

namespace SitiosWeb.Controllers
{
    public class SolicitudIncaController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public SolicitudIncaController(Tiusr22plProyectoContext context)
        {
            _context = context;
           
        }
        [HttpPost]
        public async Task<IActionResult> solicitudIncapacidades(string identificacion, string puestoLaboral, string nombres, string tipoIncapacidad, DateTime fechaInicial, DateTime fechaFinal)
        {
            var permisos = new Permisos
            {
                IdEmpleado = identificacion,

            };
            return View();

        }

    }
}
