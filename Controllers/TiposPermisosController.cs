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
    public class TiposPermisosController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public TiposPermisosController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: TiposPermisos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposPermisos.ToListAsync());
        }

        // GET: TiposPermisos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposPermisos = await _context.TiposPermisos
                .FirstOrDefaultAsync(m => m.IdTipoPermiso == id);
            if (tiposPermisos == null)
            {
                return NotFound();
            }

            return View(tiposPermisos);
        }

        // GET: TiposPermisos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposPermisos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoPermiso,Descripcion,Estado")] TiposPermisos tiposPermisos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tiposPermisos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tiposPermisos);
        }

        // GET: TiposPermisos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposPermisos = await _context.TiposPermisos.FindAsync(id);
            if (tiposPermisos == null)
            {
                return NotFound();
            }
            return View(tiposPermisos);
        }

        // POST: TiposPermisos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoPermiso,Descripcion,Estado")] TiposPermisos tiposPermisos)
        {
            if (id != tiposPermisos.IdTipoPermiso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tiposPermisos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TiposPermisosExists(tiposPermisos.IdTipoPermiso))
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
            return View(tiposPermisos);
        }

        // GET: TiposPermisos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposPermisos = await _context.TiposPermisos
                .FirstOrDefaultAsync(m => m.IdTipoPermiso == id);
            if (tiposPermisos == null)
            {
                return NotFound();
            }

            return View(tiposPermisos);
        }

        // POST: TiposPermisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tiposPermisos = await _context.TiposPermisos.FindAsync(id);
            if (tiposPermisos != null)
            {
                _context.TiposPermisos.Remove(tiposPermisos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TiposPermisosExists(int id)
        {
            return _context.TiposPermisos.Any(e => e.IdTipoPermiso == id);
        }
    }
}
