using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using SitiosWeb.Model;
using System.Runtime.Intrinsics.Arm;

namespace SitiosWeb.Controllers
{
    public class VacacionesController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public VacacionesController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> MenuVacaciones()
        {
            return View();
        }
        public async Task<IActionResult> AsignarVacacionesColectivas()
        {
            var dep = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento;
            ViewBag.VC = await _context.VacacionesColectivas.Where(u => u.IdDepartamento == dep && u.Aprobado == true).ToListAsync();
            return View(await _context.BloqueoDias.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> SolicitarVC(string inicio, string fin)
        {
            var vacacionesColectivas = new VacacionesColectivas
            {
                IdSolicitador = Request.Cookies["Id"],
                FechaInicio = DateOnly.Parse(inicio),
                FechaFin = DateOnly.Parse(fin),
                IdDepartamento = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento

            };
            _context.VacacionesColectivas.Add(vacacionesColectivas);
            await _context.SaveChangesAsync();
            return RedirectToAction("AsignarVacacionesColectivas");
        }
        public async Task<IActionResult> SeleccionarVC()
        {

            // Fetch the records where Aprobado is false and FechaInicio is earlier than the current date
            var result = await _context.VacacionesColectivas
                .Where(u => u.Aprobado == null && u.FechaInicio > DateOnly.FromDateTime(DateTime.Now.Date))
                  .ToListAsync();
            TempData["dep"] = _context.Departamentos.ToListAsync().Result;

            // Return the results to the view
            return View(result);

        }
        public async Task<IActionResult> ManejarVC(int id)
        {
            // Retrieve VacacionesColectivas matching the id
            var vacacionesColectivas = await _context.VacacionesColectivas
                .Where(u => u.IdVacaciones == id)
                .ToListAsync();

            ViewBag.VC = vacacionesColectivas;

            var vc = vacacionesColectivas.FirstOrDefault();

            if (vc != null)
            {
                var departamento = await _context.Departamentos
                    .Where(u => u.IdDepartamento == vc.IdDepartamento)
                    .FirstOrDefaultAsync();
                var jefe = await _context.Colaboradores
                    .Where(u =>  u.Identificacion == vc.IdSolicitador)
                    .FirstOrDefaultAsync();
                TempData["dep"] = departamento.NomDepartamento;
                TempData["jefe"] = jefe.Nombre + " " + jefe.Apellidos;
                TempData["inicio"] = vc.FechaInicio;
                TempData["fin"] = vc.FechaFin;
                TempData["id"] = vc.IdVacaciones;
            }

            // Return the BloqueoDias view
            return View(await _context.BloqueoDias.ToListAsync());
        }

        public async Task<IActionResult> Aprobar_DenegarVC(int id, int manejo)
        {
            var vacacionesColectivas = await _context.VacacionesColectivas
                .Where(u => u.IdVacaciones == id)
                .FirstOrDefaultAsync();

            if (manejo == 1)
            {
                vacacionesColectivas.Aprobado = true;
            }
            else
            {
                vacacionesColectivas.Aprobado = false;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("SeleccionarVC");
        }
        public async Task<IActionResult> SolicitarVacaciones()
        {
            var dep = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento;
            ViewBag.VC = await _context.VacacionesColectivas.Where(u => u.IdDepartamento == dep && u.Aprobado == true).ToListAsync();
            var solicitudes = await _context.SolicitudVacaciones.Where(u => u.IdEmpleado == Request.Cookies["Id"]).ToListAsync();
            ViewBag.DiasPasados = await _context.Vacaciones.Where(v => v.IdSolicitudNavigation.IdEmpleado == Request.Cookies["Id"] && v.IdSolicitudNavigation.Aprobadas != false && v.IdSolicitudNavigation.FechaFin > DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
            return View(await _context.BloqueoDias.ToListAsync());
        }
        public async Task<IActionResult> SolicitarVP(string solicitudVP)
        {
            if(solicitudVP == null)
            {
                TempData["Error"] = "Error al solicitar vacaciones";
                return RedirectToAction("SolicitarVacaciones");
            }
            try { 
            string[] fechas = solicitudVP.Split(",");
            var solicitud = new SolicitudVacaciones
            {
                IdEmpleado = Request.Cookies["Id"],
                TotalDias = fechas.Count(), 
                FechaFin = DateOnly.Parse(fechas.Max())
            };
            _context.SolicitudVacaciones.Add(solicitud);
                await _context.SaveChangesAsync();
            foreach (var fecha in fechas)
            {
                var fechaVacaciones = new Vacaciones
                {
                    IdSolicitud = solicitud.IdSolicitud,
                    Fecha = DateOnly.Parse(fecha)
                };
                _context.Vacaciones.Add(fechaVacaciones);
            }
                Response.Cookies.Append("Vacaciones", (double.Parse(Request.Cookies["Vacaciones"]) - fechas.Count()).ToString());
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Vacaciones solicitadas";
                return RedirectToAction("SolicitarVacaciones");
            }
            catch (Exception e) {
                TempData["Error"] = "Error al solicitar vacaciones";
                return RedirectToAction("SolicitarVacaciones");
            }
        }
    }
}
