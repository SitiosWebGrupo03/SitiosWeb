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
    public class PuestosController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public PuestosController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: Puestos
        public async Task<IActionResult> Index()
        {
            var tiusr22plProyectoContext = _context.Puestos.Include(p => p.IdDepartamentoNavigation);
            return View(await tiusr22plProyectoContext.ToListAsync());
        }

        // GET: Puestos/Detalles/5
        public async Task<IActionResult> Detalles(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puestos = await _context.Puestos
                .Include(p => p.IdDepartamentoNavigation)
                .FirstOrDefaultAsync(m => m.IdPuesto == id);
            if (puestos == null)
            {
                return NotFound();
            }

            return View(puestos);
        }

        // GET: Puestos/Create
        public IActionResult Create()
        {
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "IdDepartamento", "IdDepartamento");
            return View();
        }

        // POST: Puestos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more Detalles, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPuesto,NombrePuesto,Salario,IdDepartamento,Estado")] Puestos puestos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(puestos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "IdDepartamento", "IdDepartamento", puestos.IdDepartamento);
            return View(puestos);
        }

        // GET: Puestos/Editar/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puestos = await _context.Puestos.FindAsync(id);
            if (puestos == null)
            {
                return NotFound();
            }
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "IdDepartamento", "IdDepartamento", puestos.IdDepartamento);
            return View(puestos);
        }

        // POST: Puestos/Editar/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more Detalles, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdPuesto,NombrePuesto,Salario,IdDepartamento,Estado")] Puestos puestos)
        {
            if (id != puestos.IdPuesto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(puestos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PuestosExists(puestos.IdPuesto))
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
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "IdDepartamento", "IdDepartamento", puestos.IdDepartamento);
            return View(puestos);
        }

        // GET: Puestos/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puestos = await _context.Puestos
                .Include(p => p.IdDepartamentoNavigation)
                .FirstOrDefaultAsync(m => m.IdPuesto == id);
            if (puestos == null)
            {
                return NotFound();
            }

            return View(puestos);
        }

        // POST: Puestos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var puestos = await _context.Puestos.FindAsync(id);
            if (puestos != null)
            {
                _context.Puestos.Remove(puestos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PuestosExists(string id)
        {
            return _context.Puestos.Any(e => e.IdPuesto == id);
        }
    }
}
