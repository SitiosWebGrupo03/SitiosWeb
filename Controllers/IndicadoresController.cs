using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class IndicadoresController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public IndicadoresController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }


        public IActionResult IndexIndicadores()
        {
            dynamic inconsistenciasCount = 0, PermisosCount = 0, HorasExtrasCount = 0, MarcasCount = 0, Horas = 0, VacacionesColectivas = 0, HorasRepuestas=0, Vacaciones = 0;
            if (User.IsInRole("COLABORADOR"))
            {
                inconsistenciasCount = _context.Inconsistencias
                                       .Where(i => i.IdEmpleado == Request.Cookies["Id"])
                                       .Count();

                PermisosCount = _context.Permisos
                                           .Where(i => i.IdEmpleado == Request.Cookies["Id"])
                                           .Count();


                MarcasCount = _context.Marcas
                                           .Where(i => i.IdEmpleado == Request.Cookies["Id"])
                                           .Count();
                Horas = Math.Round(_context.Marcas
                       .Where(u => u.InicioJornada != null && u.FinJornada != null && u.IdEmpleado == Request.Cookies["Id"])
                       .AsEnumerable()
                       .Sum(u => ((DateTime)u.FinJornada - (DateTime)u.InicioJornada).TotalHours), 2);

                VacacionesColectivas = _context.VacacionesColectivas
                        .Where(i => i.IdDepartamento == int.Parse(Request.Cookies["IDDepartamento"]) && i.Aprobado == true)
                        .AsEnumerable()
                        .Sum(u => (u.FechaFin.ToDateTime(TimeOnly.MinValue) - u.FechaInicio.ToDateTime(TimeOnly.MinValue)).TotalDays);
                HorasRepuestas = _context.Reposiciones
                        .Where(i => i.Idcolaborador == Request.Cookies["Id"] && i.Apobadas == true)
                        .Sum(u=>u.HorasReponer);
                Vacaciones = _context.SolicitudVacaciones
                    .Where(i => i.IdEmpleado == Request.Cookies["Id"] && i.Aprobadas == true)
                        .Sum(u => u.TotalDias);

            }
            else if (User.IsInRole("JEFATURA"))
            {
                int id = int.Parse(Request.Cookies["IDDepartamento"]);

                inconsistenciasCount = _context.Inconsistencias
                                           .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
                                           .Count();

                PermisosCount = _context.Permisos
                                           .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
                                           .Count();


                MarcasCount = _context.Marcas
                                           .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
                                           .Count();
                Horas = Math.Round(_context.Marcas
               .Where(u => u.InicioJornada != null && u.FinJornada != null && u.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == id)
               .AsEnumerable()
               .Sum(u => ((DateTime)u.FinJornada - (DateTime)u.InicioJornada).TotalHours), 2);
                VacacionesColectivas = _context.VacacionesColectivas
                      .Where(i => i.IdDepartamento == int.Parse(Request.Cookies["IDDepartamento"]) && i.Aprobado == true)
                      .AsEnumerable()
                      .Sum(u => (u.FechaFin.ToDateTime(TimeOnly.MinValue) - u.FechaInicio.ToDateTime(TimeOnly.MinValue)).TotalDays);

                HorasRepuestas = _context.Reposiciones
                        .Where(i => i.IdcolaboradorNavigation.IdPuestoNavigation.IdDepartamento == int.Parse(Request.Cookies["IDDepartamento"]) && i.Apobadas == true)
                        .Sum(u => u.HorasReponer);
                Vacaciones = _context.SolicitudVacaciones
                        .Where(i => i.IdEmpleadoNavigation.IdPuestoNavigation.IdDepartamento == int.Parse(Request.Cookies["IDDepartamento"]) && i.Aprobadas == true)
                        .Sum(u => u.TotalDias);

            }
            else if (User.IsInRole("SUPERVISOR"))
            {
                inconsistenciasCount = _context.Inconsistencias
                                       .Count();

                PermisosCount = _context.Permisos
                                           .Count();

                MarcasCount = _context.Marcas.Count();
                Horas = Math.Round(_context.Marcas
                        .Where(u => u.InicioJornada != null && u.FinJornada != null)
                        .AsEnumerable()
                        .Sum(u => ((DateTime)u.FinJornada - (DateTime)u.InicioJornada).TotalHours), 2);
                VacacionesColectivas = _context.VacacionesColectivas
                       .Where(i => i.Aprobado==true)
                       .AsEnumerable()
                       .Sum(u => (u.FechaFin.ToDateTime(TimeOnly.MinValue) - u.FechaInicio.ToDateTime(TimeOnly.MinValue)).TotalDays);
                HorasRepuestas = _context.Reposiciones
                       .Where(i => i.Apobadas == true)
                       .Sum(u => u.HorasReponer);
                Vacaciones = _context.SolicitudVacaciones
                       .Where(i => i.Aprobadas == true)
                       .Sum(u => u.TotalDias);
            }


            ViewBag.Inconsistencias = inconsistenciasCount;
            ViewBag.Permisos = PermisosCount;
            ViewBag.Horas = Horas;
            ViewBag.DiasLaburados = MarcasCount;
            ViewBag.VacacionesColectivas = VacacionesColectivas;
            ViewBag.HorasRepuestas = HorasRepuestas;
            ViewBag.Vacaciones = Vacaciones;
            return View("~/Views/Paginas/indicadores/IndexIndicadores.cshtml");
        }
    }
}
