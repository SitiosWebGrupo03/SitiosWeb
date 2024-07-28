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
    public class InconsistenciasController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public InconsistenciasController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: Inconsistencias
        public async Task<IActionResult> Index()
        {
            var tiusr22plProyectoContext = _context.Inconsistencias.Include(i => i.IdEmpleadoNavigation).Include(i => i.IdJustificacionNavigation).Include(i => i.IdTipoInconsistenciaNavigation);
            return View(await tiusr22plProyectoContext.ToListAsync());
        }

        // GET: Inconsistencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inconsistencias = await _context.Inconsistencias
                .Include(i => i.IdEmpleadoNavigation)
                .Include(i => i.IdJustificacionNavigation)
                .Include(i => i.IdTipoInconsistenciaNavigation)
                .FirstOrDefaultAsync(m => m.IdInconsistencia == id);
            if (inconsistencias == null)
            {
                return NotFound();
            }

            return View(inconsistencias);
        }

        // GET: Inconsistencias/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            ViewData["IdJustificacion"] = new SelectList(_context.JustificacionesInconsistencias, "IdJustificacion", "IdJustificacion");
            ViewData["IdTipoInconsistencia"] = new SelectList(_context.TiposInconsistencias, "IdTipoInconsistencia", "IdTipoInconsistencia");
            return View();
        }

        // POST: Inconsistencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdInconsistencia,IdEmpleado,IdTipoInconsistencia,IdJustificacion,FechaInconsistencia")] Inconsistencias inconsistencias)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inconsistencias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", inconsistencias.IdEmpleado);
            ViewData["IdJustificacion"] = new SelectList(_context.JustificacionesInconsistencias, "IdJustificacion", "IdJustificacion", inconsistencias.IdJustificacion);
            ViewData["IdTipoInconsistencia"] = new SelectList(_context.TiposInconsistencias, "IdTipoInconsistencia", "IdTipoInconsistencia", inconsistencias.IdTipoInconsistencia);
            return View(inconsistencias);
        }

        // GET: Inconsistencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inconsistencias = await _context.Inconsistencias.FindAsync(id);
            if (inconsistencias == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", inconsistencias.IdEmpleado);
            ViewData["IdJustificacion"] = new SelectList(_context.JustificacionesInconsistencias, "IdJustificacion", "IdJustificacion", inconsistencias.IdJustificacion);
            ViewData["IdTipoInconsistencia"] = new SelectList(_context.TiposInconsistencias, "IdTipoInconsistencia", "IdTipoInconsistencia", inconsistencias.IdTipoInconsistencia);
            return View(inconsistencias);
        }

        // POST: Inconsistencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInconsistencia,IdEmpleado,IdTipoInconsistencia,IdJustificacion,FechaInconsistencia")] Inconsistencias inconsistencias)
        {
            if (id != inconsistencias.IdInconsistencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inconsistencias);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InconsistenciasExists(inconsistencias.IdInconsistencia))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", inconsistencias.IdEmpleado);
            ViewData["IdJustificacion"] = new SelectList(_context.JustificacionesInconsistencias, "IdJustificacion", "IdJustificacion", inconsistencias.IdJustificacion);
            ViewData["IdTipoInconsistencia"] = new SelectList(_context.TiposInconsistencias, "IdTipoInconsistencia", "IdTipoInconsistencia", inconsistencias.IdTipoInconsistencia);
            return View(inconsistencias);
        }

        // GET: Inconsistencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inconsistencias = await _context.Inconsistencias
                .Include(i => i.IdEmpleadoNavigation)
                .Include(i => i.IdJustificacionNavigation)
                .Include(i => i.IdTipoInconsistenciaNavigation)
                .FirstOrDefaultAsync(m => m.IdInconsistencia == id);
            if (inconsistencias == null)
            {
                return NotFound();
            }

            return View(inconsistencias);
        }

        // POST: Inconsistencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inconsistencias = await _context.Inconsistencias.FindAsync(id);
            if (inconsistencias != null)
            {
                _context.Inconsistencias.Remove(inconsistencias);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InconsistenciasExists(int id)
        {
            return _context.Inconsistencias.Any(e => e.IdInconsistencia == id);
        }
    }
}
