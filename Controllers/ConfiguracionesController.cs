﻿using System;
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

        // GET: Configuraciones/Details/5
        public async Task<IActionResult> Details(int? id)
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
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Configuraciones/Edit/5
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

        // POST: Configuraciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    }
}
