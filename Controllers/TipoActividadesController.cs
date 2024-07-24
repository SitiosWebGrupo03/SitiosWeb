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
    public class TipoActividadesController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public TipoActividadesController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: TipoActividades
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoActividades.ToListAsync());
        }

        // GET: TipoActividades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoActividades = await _context.TipoActividades
                .FirstOrDefaultAsync(m => m.IdTipoActividad == id);
            if (tipoActividades == null)
            {
                return NotFound();
            }

            return View(tipoActividades);
        }

        // GET: TipoActividades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoActividades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoActividad,NomActividad")] TipoActividades tipoActividades)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoActividades);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoActividades);
        }

        // GET: TipoActividades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoActividades = await _context.TipoActividades.FindAsync(id);
            if (tipoActividades == null)
            {
                return NotFound();
            }
            return View(tipoActividades);
        }

        // POST: TipoActividades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoActividad,NomActividad")] TipoActividades tipoActividades)
        {
            if (id != tipoActividades.IdTipoActividad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoActividades);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoActividadesExists(tipoActividades.IdTipoActividad))
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
            return View(tipoActividades);
        }

        // GET: TipoActividades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoActividades = await _context.TipoActividades
                .FirstOrDefaultAsync(m => m.IdTipoActividad == id);
            if (tipoActividades == null)
            {
                return NotFound();
            }

            return View(tipoActividades);
        }

        // POST: TipoActividades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoActividades = await _context.TipoActividades.FindAsync(id);
            if (tipoActividades != null)
            {
                _context.TipoActividades.Remove(tipoActividades);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoActividadesExists(int id)
        {
            return _context.TipoActividades.Any(e => e.IdTipoActividad == id);
        }
    }
}
