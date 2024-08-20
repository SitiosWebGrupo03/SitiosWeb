using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using SitiosWeb.Model;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace SitiosWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public LoginController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (TempData["Error"] != null)
            {
                TempData["Error"] = string.Empty;
            }

            var user = _context.Usuarios
                        .FirstOrDefault(u => u.CodUsuario == username && u.Contrasena == password && u.Estado);
            if (user == null)
            {
                TempData["Error"] = "Nombre de usuario o contraseña incorrectos.";
                return RedirectToAction("Login", "Home");
            }

            Response.Cookies.Append("Id", user.IdColaborador.ToString());
            return RedirectToAction("Mfaview", "Login");
        }
        [HttpPost]
        public async Task<IActionResult> Mfa(int codigo)
        {
            var user = _context.Usuarios
                        .Include(u => u.IdColaboradorNavigation)
                            .ThenInclude(c => c.IdPuestoNavigation)
                                .ThenInclude(p => p.IdDepartamentoNavigation)
                        .FirstOrDefault(u => u.IdColaborador == Request.Cookies["Id"]);
            //if (codigo == (int)TempData["Codigo"])
            //{
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
            
            DateOnly hireDate = user.IdColaboradorNavigation.FechaContratacion;
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            int totalMonthsWorked = ((today.Year - hireDate.Year) * 12) + today.Month - hireDate.Month;
            if (hireDate > today.AddMonths(-totalMonthsWorked))
            {
                totalMonthsWorked--;
            }
            var maxVacaciones = _context.Configuraciones.FirstOrDefault(c => c.IdConfiguraciones == 7).NumConfig;
            dynamic vacationDays = totalMonthsWorked * float.Parse(
     _context.Configuraciones.FirstOrDefault(c => c.IdConfiguraciones == 4).ValorConfig,
     CultureInfo.InvariantCulture);//se corrigio por que se estaba casteando mal 
            if (vacationDays >= maxVacaciones)
            {
                vacationDays = maxVacaciones;
            }
            
            vacationDays-= _context.SolicitudVacaciones.Where(v => v.IdEmpleado == user.IdColaborador && v.Aprobadas != false && v.FechaFin > today).Sum(U => U.TotalDias);
            Response.Cookies.Append("Nombre", nombreColaborador, options);
            Response.Cookies.Append("Rol", nombreTipoUsuario, options);
            Response.Cookies.Append("Correo", user.IdColaboradorNavigation.Correo, options);
            Response.Cookies.Append("Puesto", user.IdColaboradorNavigation.IdPuesto, options);
            Response.Cookies.Append("Departamento", user.IdColaboradorNavigation.IdPuestoNavigation.IdDepartamentoNavigation.NomDepartamento.ToString(), options);
            Response.Cookies.Append("Vacaciones", vacationDays.ToString(), options);
            Response.Cookies.Append("Id", user.IdColaborador.ToString(), options);
            return user.IdTipoUsuario switch
            {
                1 => RedirectToAction("IndexSupervisor", "Home"),
                2 => RedirectToAction("IndexJefatura", "Home"),
                3 => RedirectToAction("IndexColaborador", "Home"),
                _ => RedirectToAction("Login", "Home"),
            };
        }
        //    else
        //    {
        //        TempData["Error"] = "Código incorrecto";
        //        return RedirectToAction("Mfaview", "Login");
        //    }


        //}
        [HttpGet]
        public async Task<IActionResult> Mfaview()
        {

            //var id = Request.Cookies["Id"];
            //var user = _context.Colaboradores.FirstOrDefault(u => u.Identificacion == id);
            //var codigo = await EnviarCorreo(user.Correo);
            //TempData["Codigo"] = codigo;

            return View("~/Views/Paginas/login/Mfaview.cshtml");
        }
        private async Task<int> EnviarCorreo(string destinatario)
        {
            int codigo = new Random().Next(10000, 99999);

            // Generate the HTML content for the email
            string correo = $@"
                            <html>
                            <body>
                                <h2>Autorización de doble factor TimeMaster</h2>
                                <p>Tu código de autorización es: <strong>{codigo}</strong></p>
                                <p>Por favor, utiliza este código para completar tu autenticación.</p>
                            </body>
                            </html>";

            using (var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("timemastersco@gmail.com", "yvht erfl dcnr vryg"),
                EnableSsl = true,
            })

            using (var mailMessage = new MailMessage
            {
                From = new MailAddress("timemastersco@gmail.com"),
                Subject = "Autorización de doble factor TimeMaster",
                Body = correo,
                IsBodyHtml = true, // Set this to true to send the email as HTML
            })
            {
                mailMessage.To.Add(destinatario);

                await smtpClient.SendMailAsync(mailMessage);
            }
            return codigo;
        }


    }
}
