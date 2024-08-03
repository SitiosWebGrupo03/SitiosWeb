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
            if (string.IsNullOrEmpty(justificacionSelect)) {
                return RedirectToAction("SolicitarRepo", "Home", new { id = justificaciones });
            }
            var days = day.Split(",");
            var horas = hora.Split(",");
            var totalHoras = horas.Select(h => int.Parse(h)).ToArray();
            var repo = new Reposiciones
            {
                Idcolaborador = Request.Cookies["Id"],
                HorasReponer = totalHoras.Sum().ToString()

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
                var nombre = repo.IdcolaboradorNavigation.Nombre + " " + repo.IdcolaboradorNavigation.Apellidos;
                HttpResponseMessage response = await client.GetAsync($"?nombreEmpleado={nombre}&solicitud=reposicion&fechaInicio={days.First()}&fechaFin={days.Last()}&destinatario={Request.Cookies["Correo"]}&jefaturaDe={Request.Cookies["Departamento"]}");
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
            using HttpClient client = new HttpClient();
            {
                client.BaseAddress = new Uri("https://tiusr24pl.cuc-carrera-ti.ac.cr/correos/api/denegacion-reposiciones");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                repo.FechasReposicion.Select(x => x.DiasReposicion).ToString();
                HttpResponseMessage response = await client.GetAsync($"?nombreEmpleado={repo.IdcolaboradorNavigation.Nombre+""+ repo.IdcolaboradorNavigation.Apellidos}&fechaReposicion=ASD&detalles=ASD&destinatario={repo.IdcolaboradorNavigation.Correo}departamento={Request.Cookies["Departamento"]}");

            }
            return RedirectToAction("SelectRepos", "Home");
        }
    }
}
