using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
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
