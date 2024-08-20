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
    public class RegistroActividadesController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public RegistroActividadesController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: RegistroActividades
        public async Task<IActionResult> Index()
        {
            var actividades = await _context.RegistroActividades
                .Include(r => r.IdColaboradorNavigation)
                .Include(r => r.IdTipoActividadNavigation)
                .Include(r => r.IdValidadorNavigation)
                .ToListAsync();

            return View(actividades);
        }

        // GET: RegistroActividades/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroActividades = await _context.RegistroActividades
                .Include(r => r.IdColaboradorNavigation)
                .Include(r => r.IdTipoActividadNavigation)
                .Include(r => r.IdValidadorNavigation)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);
            if (registroActividades == null)
            {
                return NotFound();
            }

            return View(registroActividades);
        }

        // GET: RegistroActividades/Create
        public IActionResult Create()
        {
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            ViewData["IdTipoActividad"] = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad"); // Asegúrate de usar la propiedad correcta
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            return View();
        }

        // POST: RegistroActividades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRegistro,IdTipoActividad,IdColaborador,IdValidador,Observaciones,FechaActividad,DuracionEnHoras,Aprobado")] RegistroActividades registroActividades)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registroActividades);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", registroActividades.IdColaborador);
            ViewData["IdTipoActividad"] = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad", registroActividades.IdTipoActividad); // Asegúrate de usar la propiedad correcta
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", registroActividades.IdValidador);
            return View(registroActividades);
        }

        // GET: RegistroActividades/Editar/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroActividades = await _context.RegistroActividades.FindAsync(id);
            if (registroActividades == null)
            {
                return NotFound();
            }
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", registroActividades.IdColaborador);
            ViewData["IdTipoActividad"] = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad", registroActividades.IdTipoActividad); // Asegúrate de usar la propiedad correcta
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", registroActividades.IdValidador);
            return View(registroActividades);
        }

        // POST: RegistroActividades/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRegistro,IdTipoActividad,IdColaborador,IdValidador,Observaciones,FechaActividad,DuracionEnHoras,Aprobado")] RegistroActividades registroActividades)
        {
            if (id != registroActividades.IdRegistro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registroActividades);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistroActividadesExists(registroActividades.IdRegistro))
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
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", registroActividades.IdColaborador);
            ViewData["IdTipoActividad"] = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad", registroActividades.IdTipoActividad); // Asegúrate de usar la propiedad correcta
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", registroActividades.IdValidador);
            return View(registroActividades);
        }

        // GET: RegistroActividades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroActividades = await _context.RegistroActividades
                .Include(r => r.IdColaboradorNavigation)
                .Include(r => r.IdTipoActividadNavigation)
                .Include(r => r.IdValidadorNavigation)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);
            if (registroActividades == null)
            {
                return NotFound();
            }

            return View(registroActividades);
        }

        // POST: RegistroActividades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registroActividades = await _context.RegistroActividades.FindAsync(id);
            if (registroActividades != null)
            {
                _context.RegistroActividades.Remove(registroActividades);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RegistroActividades/IndexSupervisor
        public async Task<IActionResult> IndexSupervisor()
        {
            var actividadesPendientes = await _context.RegistroActividades
                .Where(r => !r.Aprobado) // Solo actividades pendientes de aprobación
                .Include(r => r.IdColaboradorNavigation)
                .Include(r => r.IdTipoActividadNavigation)
                .Include(r => r.IdValidadorNavigation)
                .ToListAsync();

            return View(actividadesPendientes);
        }

        // POST: RegistroActividades/ApproveActivity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveActivity(int id)
        {
            var actividad = await _context.RegistroActividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            actividad.Aprobado = true;
            _context.Update(actividad);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexSupervisor));
        }

        // POST: RegistroActividades/RejectActivity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectActivity(int id)
        {
            var actividad = await _context.RegistroActividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            actividad.Aprobado = false;
            _context.Update(actividad);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexSupervisor));
        }

        // GET: RegistroActividades/ControlGeneral
        public async Task<IActionResult> ControlGeneral()
        {
            var actividades = await _context.RegistroActividades
                .Include(r => r.IdColaboradorNavigation)
                .Include(r => r.IdTipoActividadNavigation)
                .ToListAsync();

            return View(actividades);
        }

        private bool RegistroActividadesExists(int id)
        {
            return _context.RegistroActividades.Any(e => e.IdRegistro == id);
        }
    }
}
