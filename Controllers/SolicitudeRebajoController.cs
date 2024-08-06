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
    public class SolicitudeRebajoController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public SolicitudeRebajoController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: SolicitudeRebajo
        public async Task<IActionResult> Index()
        {
            return View(await _context.SolicitudeRebajo.ToListAsync());
        }

        // GET: SolicitudeRebajo/Details/5
        [HttpPost]
        public async Task<IActionResult> solicitarRebajo(int idJustificacion, string observaciones)
        {
            var justificacion = await _context.JustificacionesInconsistencias
                .FirstOrDefaultAsync(j => j.IdJustificacion == idJustificacion);

            if (justificacion == null)
            {
                return NotFound();
            }

            justificacion.Validacion = false;
            _context.Update(justificacion);

            var solicitudRebajo = new SolicitudeRebajo
            {
                IdSolicitante = Request.Cookies["Id"],
                IdInconsistencia = justificacion.IdJustificacion,
                Observaciones = observaciones
            };

            _context.SolicitudeRebajo.Add(solicitudRebajo);
            await _context.SaveChangesAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> rechazarRebajo(int idJustificacion)
        {
            var justificacion = await _context.JustificacionesInconsistencias
                .FirstOrDefaultAsync(j => j.IdJustificacion == idJustificacion);

            if (justificacion == null)
            {
                return NotFound();
            }

            justificacion.Validacion = true;
            _context.Update(justificacion);

            await _context.SaveChangesAsync();

            return View();
        }


        // GET: SolicitudeRebajo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SolicitudeRebajo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSolicitud,IdSolicitante,IdInconsistencia,Observaciones")] SolicitudeRebajo solicitudeRebajo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solicitudeRebajo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(solicitudeRebajo);
        }

        // GET: SolicitudeRebajo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudeRebajo = await _context.SolicitudeRebajo.FindAsync(id);
            if (solicitudeRebajo == null)
            {
                return NotFound();
            }
            return View(solicitudeRebajo);
        }

        // POST: SolicitudeRebajo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSolicitud,IdSolicitante,IdInconsistencia,Observaciones")] SolicitudeRebajo solicitudeRebajo)
        {
            if (id != solicitudeRebajo.IdSolicitud)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solicitudeRebajo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolicitudeRebajoExists(solicitudeRebajo.IdSolicitud))
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
            return View(solicitudeRebajo);
        }

        // GET: SolicitudeRebajo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitudeRebajo = await _context.SolicitudeRebajo
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);
            if (solicitudeRebajo == null)
            {
                return NotFound();
            }

            return View(solicitudeRebajo);
        }

        // POST: SolicitudeRebajo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solicitudeRebajo = await _context.SolicitudeRebajo.FindAsync(id);
            if (solicitudeRebajo != null)
            {
                _context.SolicitudeRebajo.Remove(solicitudeRebajo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SolicitudeRebajoExists(int id)
        {
            return _context.SolicitudeRebajo.Any(e => e.IdSolicitud == id);
        }
    }
}
