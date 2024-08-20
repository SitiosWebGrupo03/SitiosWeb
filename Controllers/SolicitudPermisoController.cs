using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class SolicitudPermisoesController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public SolicitudPermisoesController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: SolicitudPermisoes
        public async Task<IActionResult> Index()
        {
            var tiusr22plProyectoContext = _context.SolicitudPermiso
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdTipoPermisoNavigation);
            return View(await tiusr22plProyectoContext.ToListAsync());
        }

        // GET: SolicitudPermisoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudPermiso = await _context.SolicitudPermiso
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdTipoPermisoNavigation)
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);
            if (solicitudPermiso == null)
            {
                return NotFound();
            }

            return View(solicitudPermiso);
        }



        // POST: SolicitudPermisoes/Create
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSolicitud,IdEmpleado,DOH,DiasHorasFuera,Comentarios,IdTipoPermiso")] SolicitudPermiso solicitudPermiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solicitudPermiso);
                await _context.SaveChangesAsync();
            }


            return View("~/Views/SolicitudPermiso/Aprobacion.cshtml", _context.SolicitudPermiso.Where(u => u.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == int.Parse(Request.Cookies["IDDepartamento"])).ToList());

        }

        // GET: SolicitudPermisoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudPermiso = await _context.SolicitudPermiso.FindAsync(id);
            if (solicitudPermiso == null)
            {
                return NotFound();
            }

            // Cargar los datos para los selectores en la vista de edición
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", solicitudPermiso.IdEmpleado);
            ViewData["IdTipoPermiso"] = new SelectList(_context.TiposPermisos, "IdTipoPermiso", "Descripcion", solicitudPermiso.IdTipoPermiso);

            return View(solicitudPermiso);
        }

        // POST: SolicitudPermisoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSolicitud,IdEmpleado,DOH,DiasHorasFuera,Comentarios,IdTipoPermiso")] SolicitudPermiso solicitudPermiso)
        {
            if (id != solicitudPermiso.IdSolicitud)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solicitudPermiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolicitudPermisoExists(solicitudPermiso.IdSolicitud))
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

            // En caso de error, volver a cargar los datos para los selectores
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", solicitudPermiso.IdEmpleado);
            ViewData["IdTipoPermiso"] = new SelectList(_context.TiposPermisos, "IdTipoPermiso", "Descripcion", solicitudPermiso.IdTipoPermiso);

            return View(solicitudPermiso);
        }

        // GET: SolicitudPermisoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudPermiso = await _context.SolicitudPermiso
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdTipoPermisoNavigation)
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);
            if (solicitudPermiso == null)
            {
                return NotFound();
            }

            return View(solicitudPermiso);
        }

        // POST: SolicitudPermisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solicitudPermiso = await _context.SolicitudPermiso.FindAsync(id);
            if (solicitudPermiso != null)
            {
                _context.SolicitudPermiso.Remove(solicitudPermiso);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SolicitudPermisoExists(int id)
        {
            return _context.SolicitudPermiso.Any(e => e.IdSolicitud == id);
        }

        // GET: SolicitudPermisoes/Aprobacion
        public async Task<IActionResult> Aprobacion()
        {
            var permisos = await _context.SolicitudPermiso
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdTipoPermisoNavigation)
                .Where(s => !s.EstaAprobado.HasValue) // Opcional: Solo mostrar pendientes
                .ToListAsync();
            return View(permisos);
        }

        // GET: SolicitudPermisoes/Approve/5
        public async Task<IActionResult> Approve(int id)
        {
            var solicitud = await _context.SolicitudPermiso.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            solicitud.EstaAprobado = true;
            _context.Update(solicitud);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Aprobacion));
        }

        // GET: SolicitudPermisoes/Reject/5
        public async Task<IActionResult> Reject(int id)
        {
            var solicitud = await _context.SolicitudPermiso.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            solicitud.EstaAprobado = false;
            _context.Update(solicitud);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Aprobacion));
        }

        // GET: SolicitudPermisoes/ReporteGeneral
        public async Task<IActionResult> ReporteGeneral()
        {
            var permisos = await _context.SolicitudPermiso
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdTipoPermisoNavigation)
                .ToListAsync();
            return View(permisos);
        }
    }
}
