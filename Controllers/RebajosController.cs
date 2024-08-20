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
    public class RebajosController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public RebajosController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: Rebajos
        public async Task<IActionResult> Index()
        {
            var tiusr22plProyectoContext = _context.Rebajos.Include(r => r.IdColaboradorNavigation).Include(r => r.IdTipoRebajoNavigation).Include(r => r.IdValidadorNavigation).Include(r => r.InconsistenciaNavigation);
            return View(await tiusr22plProyectoContext.ToListAsync());
        }


        // GET: Rebajos/Create
        public IActionResult Create()
        {
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo");
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia");
            return View();
        }

        // POST: Rebajos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRebajo,IdColaborador,IdValidador,FechaRebajo,Inconsistencia,IdTipoRebajo,Aprobacion")] Rebajos rebajos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rebajos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdColaborador);
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo", rebajos.IdTipoRebajo);
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdValidador);
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia", rebajos.Inconsistencia);
            return View(rebajos);
        }

        // GET: Rebajos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rebajos = await _context.Rebajos.FindAsync(id);
            if (rebajos == null)
            {
                return NotFound();
            }
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdColaborador);
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo", rebajos.IdTipoRebajo);
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdValidador);
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia", rebajos.Inconsistencia);
            return View(rebajos);
        }

        // POST: Rebajos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRebajo,IdColaborador,IdValidador,FechaRebajo,Inconsistencia,IdTipoRebajo,Aprobacion")] Rebajos rebajos)
        {
            if (id != rebajos.IdRebajo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rebajos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RebajosExists(rebajos.IdRebajo))
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
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdColaborador);
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo", rebajos.IdTipoRebajo);
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdValidador);
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia", rebajos.Inconsistencia);
            return View(rebajos);
        }

        // GET: Rebajos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rebajos = await _context.Rebajos
                .Include(r => r.IdColaboradorNavigation)
                .Include(r => r.IdTipoRebajoNavigation)
                .Include(r => r.IdValidadorNavigation)
                .Include(r => r.InconsistenciaNavigation)
                .FirstOrDefaultAsync(m => m.IdRebajo == id);
            if (rebajos == null)
            {
                return NotFound();
            }

            return View(rebajos);
        }
        public IActionResult HistoricoRebajos()
        {
            dynamic result=0;
            if (User.IsInRole("COLABORADOR")) 
            {
                result = _context.Rebajos.Where(r => r.IdColaborador == Request.Cookies["Id"])
                    .Include(r => r.IdTipoRebajoNavigation)
                    .ToList();
            }
            else if (User.IsInRole("JEFATURA")) 
            { 
                ViewBag.Nombres = _context.Colaboradores.ToList();
                result = _context.Rebajos.Where(r => r.IdColaboradorNavigation.IdPuestoNavigation.IdDepartamentoNavigation.IdDepartamento == int.Parse(Request.Cookies["IDDepartamento"]))
                                        .Include(r => r.IdTipoRebajoNavigation)

                    .ToList();
            }
            else if (User.IsInRole("SUPERVISOR"))
            {
                ViewBag.Nombres = _context.Colaboradores.ToList();
                result = _context.Rebajos
                    .Include(r => r.IdTipoRebajoNavigation)
                    .ToList();
               
            }
            
            return View(result);
        }
        // POST: Rebajos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rebajos = await _context.Rebajos.FindAsync(id);
            if (rebajos != null)
            {
                _context.Rebajos.Remove(rebajos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RebajosExists(int id)
        {
            return _context.Rebajos.Any(e => e.IdRebajo == id);
        }

    }
}
