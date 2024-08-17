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
    public class InconsistenciasController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public InconsistenciasController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: Inconsistencias
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(Request.Cookies["Id"]))
            {
                return BadRequest("El ID del colaborador es requerido");
            }

            var colaborador = await _context.Colaboradores
                .Include(c => c.IdPuestoNavigation)
                .ThenInclude(p => p.IdDepartamentoNavigation)
                .FirstOrDefaultAsync(c => c.Identificacion == Request.Cookies["Id"]);

            if (colaborador == null || colaborador.IdPuestoNavigation == null || colaborador.IdPuestoNavigation.IdDepartamentoNavigation == null)
            {
                return NotFound("Colaborador o departamento no encontrado");
            }

            string config = inconsistenciasPermitidas();

            ViewBag.InconsistenciasPermitidas = config;

            var departamentoId = colaborador.IdPuestoNavigation.IdDepartamentoNavigation.IdDepartamento;

            var inconsistencias = await _context.Inconsistencias
                .Include(i => i.IdEmpleadoNavigation)
                .Include(i => i.IdJustificacionNavigation)
                .Include(i => i.IdTipoInconsistenciaNavigation)
                .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamentoNavigation.IdDepartamento == departamentoId
                 && (i.Mostrar == true || i.Mostrar == null))
                .ToListAsync();

            return View(inconsistencias);
        }

        public async Task<IActionResult> IndexPorIdentificacion(string identificacion)
        {
            var tiusr22plProyectoContext = _context.Inconsistencias
               .Include(i => i.IdEmpleadoNavigation)
               .Include(i => i.IdJustificacionNavigation)
               .Include(i => i.IdTipoInconsistenciaNavigation)
               .Where(i => i.IdEmpleado == identificacion);
            return View("~/Views/Inconsistencias/InconsistenciasPorID.cshtml", await tiusr22plProyectoContext.ToListAsync());
        }

        public string inconsistenciasPermitidas()
        {
            var configuracion = _context.Configuraciones
                .FirstOrDefault(c => c.IdConfiguraciones == 2);

            if (configuracion == null || configuracion.NumConfig == null)
            {
                throw new Exception("No se encontró la configuración o NumConfig es nulo.");
            }

            return configuracion.NumConfig.Value.ToString();
        }
        public async Task<IActionResult> IndexPorNombre(string nombreEmpleado)
        {
                var tiusr22plProyectoContext = _context.Inconsistencias
                .Include(i => i.IdEmpleadoNavigation)
                .Include(i => i.IdJustificacionNavigation)
                .Include(i => i.IdTipoInconsistenciaNavigation)
                .Where(i => i.IdEmpleadoNavigation.Nombre == nombreEmpleado);
            return View("~/Views/Inconsistencias/InconsistenciasPorID.cshtml", await tiusr22plProyectoContext.ToListAsync());
        }


        public async Task<IActionResult> Justificacion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var justificacion = await _context.JustificacionesInconsistencias
                .Include(j => j.IdColaboradorNavigation)
                .Include(j => j.IdDepartamentoNavigation)
                .Include(j => j.IdPuestoNavigation)
                .Include(j => j.IdTipoInconsistenciaNavigation)
                .Include(j => j.ReposicionNavigation)
                .FirstOrDefaultAsync(m => m.IdJustificacion == id);

            if (justificacion == null)
            {
                return NotFound();
            }

            return View("~/Views/Inconsistencias/EvaluacionJustificaciones.cshtml", justificacion);
        }

        private bool InconsistenciasExists(int id)
        {
            return _context.Inconsistencias.Any(e => e.IdInconsistencia == id);
        }
    }
}
