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
    public class TiposRebajosController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public TiposRebajosController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: TiposRebajos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposRebajos.ToListAsync());
        }

        // GET: TiposRebajos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var tiposRebajos = await _context.TiposRebajos
                .Select(ti => new { ti.IdTipoRebajo, ti.Rebajos, ti.Cantidad })
                .ToListAsync();

            return Json(tiposRebajos);
        }

        // GET: TiposRebajos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposRebajos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoRebajo,Descripcion,Cantidad,Estado")] TiposRebajos tiposRebajos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tiposRebajos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tiposRebajos);
        }

        // GET: TiposRebajos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposRebajos = await _context.TiposRebajos.FindAsync(id);
            if (tiposRebajos == null)
            {
                return NotFound();
            }
            return View(tiposRebajos);
        }

        // POST: TiposRebajos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoRebajo,Descripcion,Cantidad,Estado")] TiposRebajos tiposRebajos)
        {
            if (id != tiposRebajos.IdTipoRebajo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tiposRebajos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TiposRebajosExists(tiposRebajos.IdTipoRebajo))
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
            return View(tiposRebajos);
        }

        // GET: TiposRebajos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tiposRebajos = await _context.TiposRebajos
                .FirstOrDefaultAsync(m => m.IdTipoRebajo == id);
            if (tiposRebajos == null)
            {
                return NotFound();
            }

            return View(tiposRebajos);
        }

        // POST: TiposRebajos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tiposRebajos = await _context.TiposRebajos.FindAsync(id);
            if (tiposRebajos != null)
            {
                _context.TiposRebajos.Remove(tiposRebajos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TiposRebajosExists(int id)
        {
            return _context.TiposRebajos.Any(e => e.IdTipoRebajo == id);
        }
    }
}
