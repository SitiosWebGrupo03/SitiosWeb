using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<IActionResult> cargarTablaIncapacidades()
        {
            var permisos = await _context.SolicitudPermiso.AsNoTracking().ToListAsync();


            return View("~/Views/Incapacidades/AprobacionoDeneInca.cshtml", permisos);
        }


        [HttpPost]
        public IActionResult SolicitudIncapacidad(string identificacion, string puestoLaboral, DateTime FechaInicio, DateTime FechaFin, string idTipoPermiso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuario = _context.Colaboradores.FirstOrDefault(u => u.Identificacion == identificacion);

                    if (usuario == null)
                    {
                        TempData["ErrorMessage"] = "El usuario con esa identificación no existe.";
                        ViewBag.Permisos = _context.TiposPermisos.ToList();
                        return View("~/Views/Incapacidades/SolicitudIncapacidades.cshtml");
                    }


                    int cantidadDias = (FechaFin - FechaInicio).Days;

                    var nuevaSolicitud = new SolicitudPermiso
                    {
                        IdEmpleado = identificacion,
                        puestoLaboral = puestoLaboral,
                        FechaInicio = FechaInicio,
                        FechaFin = FechaFin,
                        IdTipoPermiso = Convert.ToInt32(idTipoPermiso),
                        Comentarios = "",
                        DiasHorasFuera = cantidadDias
                    };

                    _context.SolicitudPermiso.Add(nuevaSolicitud);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Su solicitud ha sido enviada con éxito.";
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "Ocurrió un error al procesar su solicitud. Por favor, intente nuevamente más tarde.";
            }

            ViewBag.Permisos = _context.TiposPermisos.ToList();
            return View("~/Views/Incapacidades/SolicitudIncapacidades.cshtml");
        }




        [HttpGet]
        public async Task<IActionResult> cargarCBXpermisos()
        {
            try
            {
                var permisos = await _context.TiposPermisos
                    .Where(tp => tp.Estado)
                    .Select(tp => new
                    {
                        tp.IdTipoPermiso,
                        tp.Descripcion
                    })
                    .ToListAsync();

                // Asegúrate de esperar la tarea antes de asignarla a ViewBag
                ViewBag.Permisos = permisos;

                return View("~/Views/Incapacidades/SolicitudIncapacidades.cshtml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error: " + ex.Message });
            }
        }


    }
}
