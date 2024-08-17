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
            var tiusr22plProyectoContext = _context.SolicitudPermiso.Include(s => s.IdEmpleadoNavigation).Include(s => s.IdTipoPermisoNavigation);
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

        // GET: SolicitudPermisoes/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            ViewData["IdTipoPermiso"] = new SelectList(_context.TiposPermisos, "IdTipoPermiso", "IdTipoPermiso");
            return View();
        }

        // POST: SolicitudPermisoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSolicitud,IdEmpleado,DOH,DiasHorasFuera,Comentarios,IdTipoPermiso")] SolicitudPermiso solicitudPermiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solicitudPermiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", solicitudPermiso.IdEmpleado);
            ViewData["IdTipoPermiso"] = new SelectList(_context.TiposPermisos, "IdTipoPermiso", "IdTipoPermiso", solicitudPermiso.IdTipoPermiso);
            return View(solicitudPermiso);
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
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", solicitudPermiso.IdEmpleado);
            ViewData["IdTipoPermiso"] = new SelectList(_context.TiposPermisos, "IdTipoPermiso", "IdTipoPermiso", solicitudPermiso.IdTipoPermiso);
            return View(solicitudPermiso);
        }

        // POST: SolicitudPermisoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", solicitudPermiso.IdEmpleado);
            ViewData["IdTipoPermiso"] = new SelectList(_context.TiposPermisos, "IdTipoPermiso", "IdTipoPermiso", solicitudPermiso.IdTipoPermiso);
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SolicitudPermisoExists(int id)
        {
            return _context.SolicitudPermiso.Any(e => e.IdSolicitud == id);
        }
    }
}
