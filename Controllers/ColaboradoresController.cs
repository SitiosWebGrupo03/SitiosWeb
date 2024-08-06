using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    [Authorize(Roles = "Jefatura")] // Asegúrate de que solo los usuarios con el rol adecuado puedan acceder
    public class ColaboradoresController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public ColaboradoresController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: Colaboradores
        public async Task<IActionResult> Index()
        {
            var tiusr22plProyectoContext = _context.Colaboradores.Include(c => c.IdPuestoNavigation);
            return View(await tiusr22plProyectoContext.ToListAsync());
        }

        // GET: Colaboradores/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var colaboradores = await _context.Colaboradores
                .Include(c => c.IdPuestoNavigation)
                .FirstOrDefaultAsync(m => m.Identificacion == id);
            if (colaboradores == null)
            {
                return NotFound();
            }

            return View(colaboradores);
        }

        // GET: Colaboradores/Create
        public IActionResult Create()
        {
            ViewData["IdPuesto"] = new SelectList(_context.Puestos, "IdPuesto", "NombrePuesto"); // Asegúrate de usar el campo adecuado para mostrar
            return View();
        }

        // POST: Colaboradores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Identificacion,Nombre,Apellidos,FechaNacimiento,FechaContratacion,FechaFinContrato,IdPuesto")] Colaboradores colaboradores)
        {
            if (ModelState.IsValid)
            {
                _context.Add(colaboradores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPuesto"] = new SelectList(_context.Puestos, "IdPuesto", "NombrePuesto", colaboradores.IdPuesto);
            return View(colaboradores);
        }

        // GET: Colaboradores/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var colaboradores = await _context.Colaboradores.FindAsync(id);
            if (colaboradores == null)
            {
                return NotFound();
            }
            ViewData["IdPuesto"] = new SelectList(_context.Puestos, "IdPuesto", "NombrePuesto", colaboradores.IdPuesto);
            return View(colaboradores);
        }

        // POST: Colaboradores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Identificacion,Nombre,Apellidos,FechaNacimiento,FechaContratacion,FechaFinContrato,IdPuesto")] Colaboradores colaboradores)
        {
            if (id != colaboradores.Identificacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(colaboradores);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColaboradoresExists(colaboradores.Identificacion))
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
            ViewData["IdPuesto"] = new SelectList(_context.Puestos, "IdPuesto", "NombrePuesto", colaboradores.IdPuesto);
            return View(colaboradores);
        }

        // GET: Colaboradores/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var colaboradores = await _context.Colaboradores
                .Include(c => c.IdPuestoNavigation)
                .FirstOrDefaultAsync(m => m.Identificacion == id);
            if (colaboradores == null)
            {
                return NotFound();
            }

            return View(colaboradores);
        }

        // POST: Colaboradores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var colaboradores = await _context.Colaboradores.FindAsync(id);
            if (colaboradores != null)
            {
                _context.Colaboradores.Remove(colaboradores);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ColaboradoresExists(string id)
        {
            return _context.Colaboradores.Any(e => e.Identificacion == id);
        }
    }
}
