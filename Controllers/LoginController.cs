using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SitiosWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public LoginController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (TempData["Error"] != null)
            {
                TempData["Error"] = string.Empty;

            }

            var user = _context.Usuarios
                                .Include(u => u.IdColaboradorNavigation)
                                    .ThenInclude(c => c.IdPuestoNavigation)
                                        .ThenInclude(p => p.IdDepartamentoNavigation)
                                .FirstOrDefault(u => u.CodUsuario == username && u.Contrasena == password && u.Estado);
            if (user == null)
            {
                TempData["Error"] = "Nombre de usuario o contraseña incorrectos.";
                return RedirectToAction("Login", "Home");
            }
            var colaborador = _context.Colaboradores.Find(user.IdColaborador);
            var nombreColaborador = colaborador.Nombre + " " + colaborador.Apellidos;
            var nombreTipoUsuario = _context.TipoUsuario.Find(user.IdTipoUsuario).NomTipo;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nombreColaborador),
                new Claim(ClaimTypes.NameIdentifier, user.IdColaborador.ToString()),
                new Claim(ClaimTypes.Role,nombreTipoUsuario) 
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                CookieOptions options = new CookieOptions
                {
                    Expires = null
                };

            Response.Cookies.Append("Id", user.IdColaborador.ToString(), options);
            Response.Cookies.Append("Nombre", nombreColaborador, options);
            Response.Cookies.Append("Rol", nombreTipoUsuario, options);
            Response.Cookies.Append("Correo", user.IdColaboradorNavigation.Correo, options);
            Response.Cookies.Append("Puesto", user.IdColaboradorNavigation.IdPuesto, options);
            Response.Cookies.Append("Departamento", user.IdColaboradorNavigation.IdPuestoNavigation.IdDepartamentoNavigation.NomDepartamento, options);




            return user.IdTipoUsuario switch
            {
                1 => RedirectToAction("IndexSupervisor", "Home"),
                2 => RedirectToAction("IndexJefatura", "Home"),
                3 => RedirectToAction("IndexColaborador", "Home"),
                _ => RedirectToAction("Login", "Home"),
            };
        }
        
    }
}
