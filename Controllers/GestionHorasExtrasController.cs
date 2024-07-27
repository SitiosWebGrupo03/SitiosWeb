using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class GestionHorasExtrasController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public GestionHorasExtrasController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: GestionHorasExtras
        public async Task<IActionResult> Index()
        {
            var solicitudes = await _context.SolicitudHorasExtra
                .Include(s => s.IdTipoActividadNavigation)
                .Include(s => s.IdEmpleadoNavigation) // Incluye datos del empleado
                .Include(s => s.IdSolicitanteNavigation) // Incluye datos del solicitante
                .ToListAsync();

            return View(solicitudes);
        }

        // GET: GestionHorasExtras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudHorasExtra = await _context.SolicitudHorasExtra
                .Include(s => s.IdTipoActividadNavigation)
                .Include(s => s.IdEmpleadoNavigation) // Incluye datos del empleado
                .Include(s => s.IdSolicitanteNavigation) // Incluye datos del solicitante
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);

            if (solicitudHorasExtra == null)
            {
                return NotFound();
            }

            return View(solicitudHorasExtra);
        }

        // GET: GestionHorasExtras/Create
        public IActionResult Create()
        {
            ViewBag.IdSolicitante = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre");
            ViewBag.IdEmpleado = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre");
            ViewBag.IdTipoActividad = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad");

            return View();
        }

        // POST: GestionHorasExtras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSolicitud,IdSolicitante,IdEmpleado,FechaSolicitud,Horas,IdTipoActividad")] SolicitudHorasExtra solicitudHorasExtra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solicitudHorasExtra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdSolicitante = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre", solicitudHorasExtra.IdSolicitante);
            ViewBag.IdEmpleado = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre", solicitudHorasExtra.IdEmpleado);
            ViewBag.IdTipoActividad = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad", solicitudHorasExtra.IdTipoActividad);

            return View(solicitudHorasExtra);
        }

        // GET: GestionHorasExtras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudHorasExtra = await _context.SolicitudHorasExtra.FindAsync(id);
            if (solicitudHorasExtra == null)
            {
                return NotFound();
            }

            ViewBag.IdSolicitante = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre", solicitudHorasExtra.IdSolicitante);
            ViewBag.IdEmpleado = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre", solicitudHorasExtra.IdEmpleado);
            ViewBag.IdTipoActividad = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad", solicitudHorasExtra.IdTipoActividad);

            return View(solicitudHorasExtra);
        }

        // POST: GestionHorasExtras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSolicitud,IdSolicitante,IdEmpleado,FechaSolicitud,Horas,IdTipoActividad")] SolicitudHorasExtra solicitudHorasExtra)
        {
            if (id != solicitudHorasExtra.IdSolicitud)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solicitudHorasExtra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolicitudHorasExtraExists(solicitudHorasExtra.IdSolicitud))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdSolicitante = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre", solicitudHorasExtra.IdSolicitante);
            ViewBag.IdEmpleado = new SelectList(_context.Colaboradores, "IdColaborador", "Nombre", solicitudHorasExtra.IdEmpleado);
            ViewBag.IdTipoActividad = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad", solicitudHorasExtra.IdTipoActividad);

            return View(solicitudHorasExtra);
        }

        // GET: GestionHorasExtras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudHorasExtra = await _context.SolicitudHorasExtra
                .Include(s => s.IdTipoActividadNavigation)
                .Include(s => s.IdEmpleadoNavigation) // Incluye datos del empleado
                .Include(s => s.IdSolicitanteNavigation) // Incluye datos del solicitante
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);

            if (solicitudHorasExtra == null)
            {
                return NotFound();
            }

            return View(solicitudHorasExtra);
        }

        // POST: GestionHorasExtras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solicitudHorasExtra = await _context.SolicitudHorasExtra.FindAsync(id);
            if (solicitudHorasExtra != null)
            {
                _context.SolicitudHorasExtra.Remove(solicitudHorasExtra);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



        private bool SolicitudHorasExtraExists(int id)
        {
            return _context.SolicitudHorasExtra.Any(e => e.IdSolicitud == id);
        }
    }
}
