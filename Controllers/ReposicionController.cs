using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class ReposicionController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;
        public IActionResult Aprobar(string id)
        {
            _context.Reposiciones.Find(id).Apobadas =true;
            _context.Reposiciones.Find(id).AprobadasPor = User.Identity.Name;
            _context.SaveChanges();
            return RedirectToAction("/Home/SeleccinarRepo");
            
        }
        public IActionResult Denegar(string id)
        {
            _context.Reposiciones.Find(id).Apobadas = false;
            _context.Reposiciones.Find(id).AprobadasPor = User.Identity.Name;
            _context.SaveChanges();
            return RedirectToAction("/Home/SeleccinarRepo");
        }
    }
}
