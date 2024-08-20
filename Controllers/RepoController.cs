using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace SitiosWeb.Controllers
{
    public class RepoController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;
        public RepoController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SolicitarRepo(string day, string hora, string justificacionSelect)
        {
            var justificaciones = TempData["solicitud"] as string;
            if (string.IsNullOrEmpty(justificacionSelect))
            {
                return RedirectToAction("SolicitarRepo", "Home", new { id = justificaciones });
            }
            var days = day.Split(",");
            var horas = hora.Split(",");
            var totalHoras = horas.Select(h => int.Parse(h)).ToArray();
            var repo = new Reposiciones
            {
                Idcolaborador = Request.Cookies["Id"],
                HorasReponer = totalHoras.Sum()

            };

            foreach (var (dayStr, horaStr) in days.Zip(horas, (d, h) => (d, h)))
            {
                if (double.TryParse(horaStr, out double horasValue) && horasValue > 0)
                {
                    repo.FechasReposicion.Add(new FechasReposicion
                    {
                        DiasReposicion = DateOnly.TryParse(dayStr, out var dateOnly) ? dateOnly : DateOnly.MinValue,
                        HorasReposicion = horasValue
                    });
                }
            }
            _context.Reposiciones.Add(repo);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Solicitud de reposicion enviada correctamente.";
            repo.IdcolaboradorNavigation = _context.Colaboradores.Find(repo.Idcolaborador);
            var inconsistencias = _context.JustificacionesInconsistencias.Where(x => x.IdJustificacion == int.Parse(justificacionSelect));
            foreach (var item in inconsistencias)
            {
                item.Reposicion = repo.IdReposicion;
                _context.Entry(item).Property(e => e.Reposicion).IsModified = true;
            }

            _context.SaveChanges();
            using HttpClient client = new HttpClient();
            {
                client.BaseAddress = new Uri("https://tiusr24pl.cuc-carrera-ti.ac.cr/correos/api/solicitud");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync($"?nombreEmpleado={Request.Cookies["Nombre"]}&solicitud=reposicion&fechaInicio={days.First()}&fechaFin={days.Last()}&destinatario={Request.Cookies["Correo"]}&jefaturaDe={Request.Cookies["Departamento"]}");
            }
            var redireccion = justificaciones.Split(",").ToList();

            // Remove the item from the list
            redireccion.Remove(justificacionSelect);

            // Redirect with the updated list
            return RedirectToAction("SolicitarRepo", "Home", new { id = string.Join(",", redireccion) });
        }
        public IActionResult Aprobar(int id)
        {
            var repo = _context.Reposiciones.FirstOrDefault(x => x.IdReposicion == id);
            var idAprobador = User.FindFirstValue(ClaimTypes.NameIdentifier);
            repo.Apobadas = true;
            repo.AprobadasPor = idAprobador;
            TempData["SuccessMessage"] = "Solicitud de reposicion aprobada.";
            _context.SaveChanges();
            return RedirectToAction("SelectRepos", "Home");

        }
        public async Task<IActionResult> Denegar(int id)
        {

            var repo = _context.Reposiciones.FirstOrDefault(x => x.IdReposicion == id);
            var idAprobador = User.FindFirstValue(ClaimTypes.NameIdentifier);
            repo.Apobadas = false;
            repo.AprobadasPor = idAprobador;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Solicitud de reposicion denegada.";
            using HttpClient client = new HttpClient();
            {
                client.BaseAddress = new Uri("https://tiusr24pl.cuc-carrera-ti.ac.cr/correos/api/denegacion-reposiciones");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                repo.FechasReposicion.Select(x => x.DiasReposicion).ToString();
                HttpResponseMessage response = await client.GetAsync($"?nombreEmpleado={repo.IdcolaboradorNavigation.Nombre + "" + repo.IdcolaboradorNavigation.Apellidos}&fechaReposicion=ASD&detalles=ASD&destinatario={repo.IdcolaboradorNavigation.Correo}departamento={Request.Cookies["Departamento"]}");

            }
            return RedirectToAction("SelectRepos", "Home");
        }
        public async Task<IActionResult> SolicitudTercero(string colaboradores, string inconsistencia)
        {
            var justificaciones = TempData["solicitud"] as string;
            var inconsistencias = _context.JustificacionesInconsistencias.Where(x => x.IdJustificacion == int.Parse(inconsistencia));
            var redireccion = justificaciones.Split(",").ToList();
            int horas=0;
            redireccion.Remove(inconsistencia);
            foreach (var item in inconsistencias)
            {
                if (item.IdTipoInconsistencia == 1) { horas = 2; }
                else if (item.IdTipoInconsistencia == 2) { horas = 3; } 
            }
            

            TempData["SuccessMessage"] = "Solicitud de reposicion enviada correctamente.";
            var terceros = _context.ReposicionTercero.Add(new ReposicionTercero
            {
                Idtercero = colaboradores,
                Idsolicitante = Request.Cookies["Id"],
                Justificacion = int.Parse(inconsistencia),
                Horas = horas
            });
            _context.SaveChanges();
            return RedirectToAction("SolicitarRepo", "Home", new { id = string.Join(",", redireccion) });
        }
        public async Task<IActionResult> SolicitarTercero(string day, string hora, string justificacion)
        {
            var days = day.Split(",");
            var horas = hora.Split(",");
            var totalHoras = horas.Select(h => int.Parse(h)).ToArray();
            var repo = new Reposiciones
            {
                Idcolaborador = Request.Cookies["Id"],
                HorasReponer = totalHoras.Sum(),
                PorTercero = true
            };
            _context.Reposiciones.Add(repo);
            _context.SaveChanges();
            var justificaciones = _context.JustificacionesInconsistencias.Where(x => x.IdJustificacion == int.Parse(justificacion));
            foreach (var item in justificaciones)
            {
                item.Reposicion = repo.IdReposicion;
                _context.Entry(item).Property(e => e.Reposicion).IsModified = true;
                _context.SaveChanges();
            }
            foreach (var (dayStr, horaStr) in days.Zip(horas, (d, h) => (d, h)))
            {
                if (double.TryParse(horaStr, out double horasValue) && horasValue > 0)
                {
                    repo.FechasReposicion.Add(new FechasReposicion
                    {
                        DiasReposicion = DateOnly.TryParse(dayStr, out var dateOnly) ? dateOnly : DateOnly.MinValue,
                        HorasReposicion = horasValue
                    });
                }
            }

            TempData["SuccessMessage"] = "Solicitud de reposicion por tercero enviada correctamente.";

            _context.SaveChanges();
            using HttpClient client = new HttpClient();
            {
                client.BaseAddress = new Uri("https://tiusr24pl.cuc-carrera-ti.ac.cr/correos/api/solicitud");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync($"?nombreEmpleado={Request.Cookies["Nombre"]}&solicitud=reposicion por tercero&fechaInicio={days.First()}&fechaFin={days.Last()}&destinatario={Request.Cookies["Correo"]}&jefaturaDe={Request.Cookies["Departamento"]}");
            }

            return RedirectToAction("IndexColaborador", "Home");
            
        }
        public async Task<IActionResult> AceptarTercero(string justificacion, string action)
        {
            var tercero = await _context.ReposicionTercero.FirstOrDefaultAsync(x => x.Justificacion == int.Parse(justificacion));
            if (action == "Rechazar")
            {
                tercero.Aceptado = false;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Solicitud de reposicion por tercero denegada.";
                return RedirectToAction("IndexColaborador", "Home");
            }
            else if (action == "Aceptar")
            {
                tercero.Aceptado = true;
                tercero.IdterceroNavigation = _context.Colaboradores.Find(tercero.Idtercero);
                _context.SaveChanges();
                var dep = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento;
                ViewBag.VC = await _context.VacacionesColectivas.Where(u => u.IdDepartamento == dep && u.Aprobado == true).ToListAsync();
                ViewBag.DiasBlock = await _context.BloqueoDias.ToListAsync();
                ViewBag.DiasPasados = await _context.Vacaciones.Where(v => v.IdSolicitudNavigation.IdEmpleado == Request.Cookies["Id"] && v.IdSolicitudNavigation.Aprobadas != false && v.IdSolicitudNavigation.FechaFin > DateOnly.FromDateTime(DateTime.Now)).ToListAsync();


                return View("~/Views/Paginas/reposiciones/solicitudTercero.cshtml", tercero);

            }
            else
            {

                return RedirectToAction("IndexColaborador", "Home");
            }

        }

    }
}
