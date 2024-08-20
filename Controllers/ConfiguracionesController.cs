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
    public class ConfiguracionesController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public ConfiguracionesController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: Configuraciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.Configuraciones.ToListAsync());
        }

        // GET: Configuraciones/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configuraciones = await _context.Configuraciones
                .FirstOrDefaultAsync(m => m.IdConfiguraciones == id);
            if (configuraciones == null)
            {
                return NotFound();
            }

            return View(configuraciones);
        }

        // GET: Configuraciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Configuraciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more Detalles, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConfiguraciones,NomConfiguracion,ValorConfig,NumConfig")] Configuraciones configuraciones)
        {
            if (ModelState.IsValid)
            {
                _context.Add(configuraciones);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(configuraciones);
        }

        // GET: Configuraciones/Editar/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configuraciones = await _context.Configuraciones.FindAsync(id);
            if (configuraciones == null)
            {
                return NotFound();
            }
            return View(configuraciones);
        }

        // POST: Configuraciones/Editar/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more Detalles, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdConfiguraciones,NomConfiguracion,ValorConfig,NumConfig")] Configuraciones configuraciones)
        {
            if (id != configuraciones.IdConfiguraciones)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(configuraciones);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConfiguracionesExists(configuraciones.IdConfiguraciones))
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
            return View(configuraciones);
        }

        // GET: Configuraciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configuraciones = await _context.Configuraciones
                .FirstOrDefaultAsync(m => m.IdConfiguraciones == id);
            if (configuraciones == null)
            {
                return NotFound();
            }

            return View(configuraciones);
        }

        // POST: Configuraciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var configuraciones = await _context.Configuraciones.FindAsync(id);
            if (configuraciones != null)
            {
                _context.Configuraciones.Remove(configuraciones);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConfiguracionesExists(int id)
        {
            return _context.Configuraciones.Any(e => e.IdConfiguraciones == id);
        }
        public async Task<IActionResult> BloquearDias()
        {
            return View(await _context.BloqueoDias.ToListAsync());
        }
        public async Task<IActionResult> ManejarDias(int id, string descripcion, string tipo, string dia)
        {
            BloqueoDias ids = null;
            if (id != null)
            {
                ids = _context.BloqueoDias.FirstOrDefault(c => c.DayId == id);
            }

            switch (tipo)
            {
                case "0":
                    _context.BloqueoDias.Add(new BloqueoDias
                    {
                        Descripcion = descripcion,
                        Tipo = 0,
                        Day = DateOnly.Parse(dia)
                    });
                    TempData["Success"] = "Dia bloqueado por la empresa correctamente.";

                    break;
                case "1":
                    _context.BloqueoDias.Add(new BloqueoDias
                    {
                        Descripcion = descripcion,
                        Tipo = 1,
                        Day = DateOnly.Parse(dia)
                    });
                    TempData["Success"] = "Dia festivo agregado correctamente.";


                    break;

                case "2":
                    if (ids != null)
                    {
                        ids.Descripcion = descripcion;
                        _context.BloqueoDias.Update(ids);
                    }
                    TempData["Success"] = "Dia actualizado correctamente.";
                    break;

                case "3":
                    if (ids != null)
                    {
                        _context.BloqueoDias.Remove(ids);
                    }
                    TempData["Success"] = "Dia eliminado correctamente.";

                    break;

                default:
                    break;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("BloquearDias");
        }

    }
}
