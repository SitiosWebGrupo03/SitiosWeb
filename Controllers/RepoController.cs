using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Net.Http.Headers;
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
        public IActionResult Aprobar(int id)
        {
            var repo = _context.Reposiciones.FirstOrDefault(x => x.IdReposicion == id);
            var idAprobador = User.FindFirstValue(ClaimTypes.NameIdentifier);
            repo.Apobadas =true;
            repo.AprobadasPor = idAprobador;
            _context.SaveChanges();
            return RedirectToAction("SelectRepos", "Home");
            
        }
        public async Task<IActionResult> SolicitarRepo(string day, string hora)
        {
            var days = day.Split(",");
            var horas = hora.Split(",");
            var totalHoras = horas.Select(h => int.Parse(h)).ToArray();
            var repo = new Reposiciones
            {
                // Id se generará automáticamente por la base de datos cuando se guarde la entidad.
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

            using HttpClient client = new HttpClient();
            {
                client.BaseAddress = new Uri("https://tiusr24pl.cuc-carrera-ti.ac.cr/correos/api/solicitud");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var nombre = repo.IdcolaboradorNavigation.Nombre +" "+ repo.IdcolaboradorNavigation.Apellidos;
                HttpResponseMessage response = await client.GetAsync($"?nombreEmpleado={nombre}&solicitud=reposicion&fechaInicio={days.First()}&fechaFin={days.Last()}&destinatario={Request.Cookies["Correo"]}&jefaturaDe={Request.Cookies["Departamento"]}");
                
            }
            return RedirectToAction("SolicitarRepo", "Home");
        }
        public IActionResult Denegar(int id)
        {
            var repo = _context.Reposiciones.FirstOrDefault(x => x.IdReposicion == id);
            var idAprobador = User.FindFirstValue(ClaimTypes.NameIdentifier);
            repo.Apobadas = false;
            repo.AprobadasPor = idAprobador;
            _context.SaveChanges();
            
            return RedirectToAction("SelectRepos", "Home");
        }
    }
}
