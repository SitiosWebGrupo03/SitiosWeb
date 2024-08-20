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
    public class TiposInconsistenciasController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public TiposInconsistenciasController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: TiposInconsistencias
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposInconsistencias.ToListAsync());
        }

        // GET: TiposInconsistencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposInconsistencias = await _context.TiposInconsistencias
                .FirstOrDefaultAsync(m => m.IdTipoInconsistencia == id);
            if (tiposInconsistencias == null)
            {
                return NotFound();
            }

            return View(tiposInconsistencias);
        }

        // GET: TiposInconsistencias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposInconsistencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoInconsistencia,Descripcion,Estado")] TiposInconsistencias tiposInconsistencias)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tiposInconsistencias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tiposInconsistencias);
        }

        // GET: TiposInconsistencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposInconsistencias = await _context.TiposInconsistencias.FindAsync(id);
            if (tiposInconsistencias == null)
            {
                return NotFound();
            }
            return View(tiposInconsistencias);
        }

        // POST: TiposInconsistencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoInconsistencia,Descripcion,Estado")] TiposInconsistencias tiposInconsistencias)
        {
            if (id != tiposInconsistencias.IdTipoInconsistencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tiposInconsistencias);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TiposInconsistenciasExists(tiposInconsistencias.IdTipoInconsistencia))
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
            return View(tiposInconsistencias);
        }

        // GET: TiposInconsistencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposInconsistencias = await _context.TiposInconsistencias
                .FirstOrDefaultAsync(m => m.IdTipoInconsistencia == id);
            if (tiposInconsistencias == null)
            {
                return NotFound();
            }

            return View(tiposInconsistencias);
        }

        // POST: TiposInconsistencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tiposInconsistencias = await _context.TiposInconsistencias.FindAsync(id);
            if (tiposInconsistencias != null)
            {
                _context.TiposInconsistencias.Remove(tiposInconsistencias);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TiposInconsistenciasExists(int id)
        {
            return _context.TiposInconsistencias.Any(e => e.IdTipoInconsistencia == id);
        }
    }
}
