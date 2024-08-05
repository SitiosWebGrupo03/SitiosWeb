using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SitiosWeb.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;

namespace SitiosWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Tiusr22plProyectoContext _context;

        public HomeController(ILogger<HomeController> logger, Tiusr22plProyectoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity != null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            }
            return View();
        }

        public IActionResult Login()
        {
            return View("~/Views/Paginas/login/login.cshtml");
        }

        public IActionResult CerrarSesion()
        {

            HttpContext.SignOutAsync();

            // Clear all cookies
            foreach (var cookie in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.Response.Cookies.Delete(cookie);
            }

            // Clear TempData
            TempData.Clear();
            return View("~/Views/Home/Index.cshtml");
        }


        [Authorize(Roles = "COLABORADOR")]
        public IActionResult IndexColaborador()
        {
            var user = _context.Usuarios
                                .Include(u => u.IdColaboradorNavigation)
                                    .ThenInclude(c => c.IdPuestoNavigation)
                                        .ThenInclude(p => p.IdDepartamentoNavigation)
                                .FirstOrDefault(u => u.IdColaboradorNavigation.Identificacion == Request.Cookies["Id"]);

            try
            {
                var reposicionTerceroList = _context.ReposicionTercero.Where(x => x.Idtercero == user.IdColaborador && x.Aceptado != true && x.Aceptado != false)
                                            .Include(x => x.IdsolicitanteNavigation)
                                            .ToList();

                if (reposicionTerceroList.Any())
                {
                    TempData["tercero"] = reposicionTerceroList;
                }

                user.IdColaboradorNavigation.JustificacionesInconsistencias = _context.JustificacionesInconsistencias.ToList();
                user.IdColaboradorNavigation.Inconsistencias = _context.Inconsistencias.ToList();
            }
            catch
            {
                return View("~/Views/Paginas/Menu/menuColaborador.cshtml", user);
            }

            return View("~/Views/Paginas/Menu/menuColaborador.cshtml", user);


        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult IndexJefatura()
        {
            return View("~/Views/Paginas/Menu/menuJefatura.cshtml");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult IndexMenuInconsistencias()
        {
            return View("~/Views/Inconsistencias/Index.cshtml");
        }

        [Authorize(Roles = "COLABORADOR")]
        public IActionResult ConsularHorarioColaborador()
        {
            return View("~/Views/Shared/HorarioColab.cshtml");
        }
        [Authorize(Roles = "COLABORADOR")]
        public IActionResult MarcaNormalColab()
        {
            return View("~/Views/Marcas/MarcaNormalColab.cshtml");
        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult ConsultarHorarioJefatura()
        {
            return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");
        }


        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexSupervisor()
        {
            return View("~/Views/Paginas/Menu/menuSup.cshtml");
        }
        public IActionResult AccesoDenegado()
        {
            return View("AccesoDenegado");
        }
        [Authorize(Roles = "COLABORADOR")]

        public IActionResult indicadoresColab()
        {
            return View("~/Views/Paginas/indicadores/indicadorescolaborador.cshtml");
        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult Expediente()
        {
            return View("~/Views/ExpedienteEmpleado/AgregarC.cshtml");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult ConsultarHorario()
        {
            return View("~/Views/ExpedienteEmpleado/ConsultarHorarioJ.cshtml");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult MarcaNormal()
        {
            return View("~/Views/Marcas/MarcaNormal.cshtml");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult FaceIndex()
        {
            return View("~/Views/Marcas/MarcarFaceID.cshtml");
        }
        [Authorize(Roles = "JEFATURA")]

        [Authorize(Roles = "JEFATURA")]
        public IActionResult AsignarPuesto()
        {
            return View("~/Views/ExpedienteEmpleado/AsignarPuesto.cshtml");
        }





        public IActionResult VistaInconsistencias()
        {
            var inconsistencias = _context.Inconsistencias
                                          .Include(i => i.IdEmpleadoNavigation)
                                          .Include(i => i.IdEmpleadoNavigation.Nombre)
                                          .Include(i => i.IdJustificacionNavigation)
                                          .Include(i => i.IdTipoInconsistenciaNavigation.Descripcion)
                                          .ToList();

            return View("~/Views/Inconsistencias/Index.cshtml", inconsistencias);
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult menuInconsistencias()
        {
            return View("~/Views/Home/menuInconsistencias.cshtml");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult SelectRepos()
        {
            var reposiciones = _context.Reposiciones
                                          .Include(r => r.IdcolaboradorNavigation)
                                          .Include(r => r.FechasReposicion)
                                          .Where(r => r.Apobadas == null && r.IdcolaboradorNavigation.IdPuestoNavigation.IdDepartamentoNavigation.NomDepartamento == Request.Cookies["Departamento"])
                                          .ToList();
            
            return View("~/Views/Paginas/reposiciones/seleccionarRepo.cshtml", reposiciones);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult menuCRUD()
        {
            return View("~/Views/Home/menuCRUD.cshtml");
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexActividades()
        {
            var actividades = _context.TipoActividades.ToList();

            return View("~/Views/TipoActividades/Index.cshtml", actividades);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexUsuarios()
        {
            var usuarios = _context.TipoUsuario.ToList();

            return View("~/Views/TipoUsuarios/Index.cshtml", usuarios);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexDepartamentos()
        {
            var departamentos = _context.Departamentos.ToList();

            return View("~/Views/Departamentos/Index.cshtml", departamentos);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexPuestos()
        {
            var puestos = _context.Puestos.ToList();

            return View("~/Views/Puestos/Index.cshtml", puestos);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexInconsistencias()
        {
            var inconsistencias = _context.TiposInconsistencias.ToList();

            return View("~/Views/TiposInconsistencias/Index.cshtml", inconsistencias);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexRebajos()
        {
            var rebajos = _context.TiposRebajos.ToList();

            return View("~/Views/TiposRebajos/Index.cshtml", rebajos);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexPermisos()
        {
            var permisos = _context.TiposPermisos.ToList();

            return View("~/Views/TiposPermisos/Index.cshtml", permisos);
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult SelectRepo(int id)
        {
            var reposicion = _context.FechasReposicion
                             .Include(r => r.IdReposicionNavigation)
                             .Include(r => r.IdReposicionNavigation.IdcolaboradorNavigation)
                             .Where(r => r.IdReposicion == id)
                             .ToList();

            return View("/Views/Paginas/reposiciones/aprobacionRepo.cshtml", reposicion);
        }
        [Authorize(Roles = "COLABORADOR")]
        public IActionResult SolicitarRepo(string id)
        {
            if (id == null)
            {
                id = 0.ToString();
            }
            TempData["solicitud"] = id;
            var reposicionesList = id.Split(',').Select(int.Parse).ToList();
            var terceros = _context.Colaboradores
                .Where(c =>
                    c.IdPuestoNavigation.IdDepartamentoNavigation.NomDepartamento == Request.Cookies["Departamento"] &&
                    c.Usuarios.Any(x => x.IdTipoUsuario == 3 ) &&
                    c.Identificacion != Request.Cookies["Id"])
                .ToList();
            ViewBag.Tercero = terceros;

            var repos = _context.JustificacionesInconsistencias
                .Where(r => reposicionesList.Contains(r.IdJustificacion) && r.Reposicion == null)
                .ToList();

            return View("/Views/Paginas/reposiciones/solicitarRepo.cshtml", repos);
        }

        [Authorize(Roles = "COLABORADOR")]
        public IActionResult SolicitarHorasExtras()
        {
            // Asegúrate de proporcionar una lista de tipos de actividades si es necesario
            ViewBag.TipoActividades = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad");
            return View("~/Pages/Gestion_Horas_Extras/SolicitarHorasExtras.cshtml");
        }
        [Authorize(Roles = "JEFATURA")]
        public IActionResult SolicitarHorasExtras(SolicitudHorasExtra solicitud)
        {
            if (ModelState.IsValid)
            {
                solicitud.Estado = "Pendiente";
                _context.SolicitudHorasExtra.Add(solicitud);
                _context.SaveChanges();
                return RedirectToAction("IndexColaborador");
            }
            ViewBag.TipoActividades = new SelectList(_context.TipoActividades, "IdTipoActividad", "NomActividad");
            return View(solicitud);
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult ReporteHorasExtras()
        {
            var solicitudes = _context.SolicitudHorasExtra
                                      .Include(s => s.IdSolicitanteNavigation)
                                      .Include(s => s.IdTipoActividadNavigation)
                                      .Where(s => s.Estado == "Pendiente")
                                      .ToList();

            return View("/Views/Paginas/Gestion_Horas_Extras/ReporteHorasExtras.cshtml", solicitudes);
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult AprobarHorasExtras(int id)
        {
            var solicitud = _context.SolicitudHorasExtra.Find(id);
            if (solicitud != null)
            {
                solicitud.Estado = "Aprobada";
                solicitud.AprobadaPor = User.Identity.Name; // O cualquier lógica para asignar quien aprobó
                _context.SaveChanges();
            }
            return RedirectToAction("ReporteHorasExtras");
        }

        [Authorize(Roles = "COLABORADOR")]
        public IActionResult AprobarSolicitud(int id)
        {
            var solicitud = _context.SolicitudHorasExtra.Find(id);
            if (solicitud != null)
            {
                solicitud.Estado = "Aprobada";
                _context.SaveChanges();
            }
            return RedirectToAction("IndexColaborador");
        }
    }
}


