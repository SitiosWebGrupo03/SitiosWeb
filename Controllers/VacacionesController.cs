using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using SitiosWeb.Model;
using System.Linq;
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
            try {
                var vacacionesColectivas = new VacacionesColectivas
                {
                    IdSolicitador = Request.Cookies["Id"],
                    FechaInicio = DateOnly.Parse(inicio),
                    FechaFin = DateOnly.Parse(fin),
                    IdDepartamento = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento

                };
                _context.VacacionesColectivas.Add(vacacionesColectivas);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Vacaciones colectivas solicitadas";
                return RedirectToAction("AsignarVacacionesColectivas");
            }
            catch (Exception e) {
                TempData["Error"] = "Error al solicitar vacaciones colectivas";
                return RedirectToAction("AsignarVacacionesColectivas");
            }

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
                    .Where(u => u.Identificacion == vc.IdSolicitador)
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
            TempData["Success"] = $"Vacaciones colectivas {(manejo == 1 ? "Aprobadas" : "Denegadas")} con exito";
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
            if (solicitudVP == null)
            {
                TempData["Error"] = "Error al solicitar vacaciones";
                return RedirectToAction("SolicitarVacaciones");
            }
            try
            {
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
            catch (Exception e)
            {
                TempData["Error"] = "Error al solicitar vacaciones";
                return RedirectToAction("SolicitarVacaciones");
            }
        }
        public async Task<IActionResult> SeleccionarVacaciones()
        {
            dynamic result;
            ViewBag.Nombres = await _context.Colaboradores.ToListAsync();
            if (User.IsInRole("JEFATURA"))
            {
                result = await _context.SolicitudVacaciones
               .Where(u => u.Aprobadas == null && u.IdEmpleadoNavigation.Usuarios.Any(usuario => usuario.IdTipoUsuario == 3))
               .ToListAsync();

                return View(result);

            }
            else if (User.IsInRole("SUPERVISOR"))
            {
                result = await _context.SolicitudVacaciones
                 .Where(u => u.Aprobadas == null)
                 .ToListAsync();
                return View(result);
            }
            else
            {
                return RedirectToAction("MenuVacaciones");
            }
        }
        public async Task<IActionResult> AprobarVacaciones(int id, string name)
        {

            ViewBag.Solicitud = await _context.SolicitudVacaciones
                .Include(u => u.Vacaciones)
                .Where(u => u.IdSolicitud == id)
                .ToListAsync();
            ViewBag.Nombre = name;
            ViewBag.Id = id;

            if (User.IsInRole("JEFATURA"))
            {
                var dep = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento;
                ViewBag.VC = await _context.VacacionesColectivas.Where(u => u.IdDepartamento == dep && u.Aprobado == true).ToListAsync();
            }
            else if (User.IsInRole("SUPERVISOR"))
            {
                ViewBag.VC = await _context.VacacionesColectivas.Where(u => u.IdDepartamento == u.IdSolicitadorNavigation.IdPuestoNavigation.IdDepartamento && u.Aprobado == true).ToListAsync();
            }
            return View(await _context.BloqueoDias.ToListAsync());
        }
        public async Task<IActionResult> Aprobar_Denegar(int id, int manejo, string solicitudVP)
        {
            string[] fechas = solicitudVP.Split(",");
            var fechasList = fechas.Select(f => DateOnly.Parse(f)).ToList();
            var fechasToRemove = await _context.Vacaciones
                .Where(v => v.IdSolicitud == id && !fechasList.Contains(v.Fecha))
                .ToListAsync();
            _context.Vacaciones.RemoveRange(fechasToRemove);

            await _context.SaveChangesAsync();

            var solicitud = await _context.SolicitudVacaciones
                .Where(u => u.IdSolicitud == id)
                .FirstOrDefaultAsync();

            if (solicitud == null)
            {
                return NotFound();
            }

            solicitud.AprobadasPor = Request.Cookies["Nombre"];
            solicitud.TotalDias = fechas.Length;
            solicitud.Aprobadas = (manejo == 1);
            solicitud.FechaFin = _context.Vacaciones.Where(c=>c.IdSolicitud==id).ToList().Max(c=>c.Fecha);
            TempData["Success"] = $"Vacaciones {(manejo == 1 ? "Aprobadas" : "Denegadas")} con exito";


            // Save changes to update the solicitud object
            await _context.SaveChangesAsync();

            // Convert the fechas to a list of dates

            // Remove the vacation dates from the Vacaciones table
            

            
            return RedirectToAction("SeleccionarVacaciones");
        }

    }
}
