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
            var colaboradores = _context.Colaboradores
                .Include(c => c.IdPuestoNavigation)
                    .ThenInclude(p => p.IdDepartamentoNavigation)
                    .Include(c => c.Usuarios)
                    .Include(c => c.Marcas)
                    .Include(c => c.Inconsistencias)
                .ToList();
            return View(colaboradores);
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
            return RedirectToAction("Index", "Home");
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

        [Authorize(Roles = "JEFATURA,SUPERVISOR")]
        public IActionResult ImpactoMonetario()
        {
            return View("~/Views/Incapacidades/ImpactoMometario.cshtml");
        }


        [Authorize(Roles = "COLABORADOR")]

        public IActionResult MarcarHEX()
        {
            return View("~/Views/Marcas/MarcarHEX.cshtml");
        }
        [Authorize(Roles = "COLABORADOR")]
        public IActionResult FaceIDColab()
        {
            return View("~/Views/Marcas/MarcarFaceID.cshtml");
        }


        [Authorize(Roles = "JEFATURA")]

        public IActionResult CrearUs()
        {
            return View("~/Views/Shared/CreacionUsuario.cshtml");
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
        [Authorize(Roles = "JEFATURA,SUPERVISOR")]
        public IActionResult AprobacionIncapacidades()
        {
            return View("~/Views/Incapacidades/AprobacionoDeneInca.cshtml");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult ConsultarHorarioJefatura()
        {
            return View("~/Views/ExpedienteEmpleado/HorarioVistaJef.cshtml");
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
        public IActionResult AsignarPuesto()
        {
            return View("~/Views/ExpedienteEmpleado/AsignarPuesto.cshtml");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult AsignarPuestos()
        {
            return View("~/Views/PuestoTrabajo/AsignarPuesto.cshtml");
        }


        [Authorize(Roles = "JEFATURA")]
        public IActionResult Colaboradores()
        {
            var colaboradores = _context.Colaboradores
                                        .Include(c => c.IdPuestoNavigation)
                                        .ToList();
            return View("~/Views/PuestoTrabajo/Colaboradores.cshtml", colaboradores);
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult IndexConfig()
        {
            var configuraciones = _context.Configuraciones.ToList();
            return View("~/Views/Configuraciones/Index.cshtml", configuraciones);
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
        [Authorize(Roles = "JEFATURA,COLABORADOR,SUPERVISOR")]
        public IActionResult SolicitudIncapacidad()
        {
            return View("~/Views/Incapacidades/SolicitudIncapacidades.cshtml");
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
        public async Task<IActionResult> SelectRepo(int id)
        {
            var reposicion = _context.FechasReposicion
                             .Include(r => r.IdReposicionNavigation)
                             .Include(r => r.IdReposicionNavigation.IdcolaboradorNavigation)
                             .Include(r => r.IdReposicionNavigation.FechasReposicion)
                             .Include(r => r.IdReposicionNavigation.IdcolaboradorNavigation.IdPuestoNavigation)
                             .Include(r => r.IdReposicionNavigation.IdcolaboradorNavigation.IdPuestoNavigation.HorariosXPuesto)
                             .Include(r => r.IdReposicionNavigation.IdcolaboradorNavigation.ReposicionTerceroIdterceroNavigation)
                             .Include(r => r.IdReposicionNavigation.JustificacionesInconsistencias)
                             .Where(r => r.IdReposicion == id)
                             .ToList();
         ViewBag.colaboradores = _context.Colaboradores
                                 .Where(c => c.IdPuestoNavigation.IdDepartamentoNavigation.NomDepartamento == Request.Cookies["Departamento"])
                                 .ToList();
            var dep = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento;
            ViewBag.VC = await _context.VacacionesColectivas.Where(u => u.IdDepartamento == dep && u.Aprobado == true).ToListAsync();
            ViewBag.DiasBlock = await _context.BloqueoDias.ToListAsync();
            ViewBag.DiasPasados = await _context.Vacaciones.Where(v => v.IdSolicitudNavigation.IdEmpleado == _context.Colaboradores.FirstOrDefault(c=>c.Identificacion == _context.Reposiciones.FirstOrDefault(c=>c.IdReposicion==id).Idcolaborador).Identificacion && v.IdSolicitudNavigation.Aprobadas != false && v.IdSolicitudNavigation.FechaFin > DateOnly.FromDateTime(DateTime.Now)).ToListAsync();


            return View("/Views/Paginas/reposiciones/aprobacionRepo.cshtml", reposicion);
        }
        [Authorize(Roles = "COLABORADOR")]
        public async Task<IActionResult> SolicitarRepo(string id)
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
                    c.Usuarios.Any(x => x.IdTipoUsuario == 3) &&
                    c.Identificacion != Request.Cookies["Id"])
                .ToList();
            ViewBag.Tercero = terceros;

            var dep = _context.Departamentos.Where(u => u.NomDepartamento == Request.Cookies["Departamento"]).FirstOrDefaultAsync().Result.IdDepartamento;
            ViewBag.VC = await _context.VacacionesColectivas.Where(u => u.IdDepartamento == dep && u.Aprobado == true).ToListAsync();
            ViewBag.DiasBlock = await _context.BloqueoDias.ToListAsync();
            ViewBag.DiasPasados = await _context.Vacaciones.Where(v => v.IdSolicitudNavigation.IdEmpleado == Request.Cookies["Id"] && v.IdSolicitudNavigation.Aprobadas != false && v.IdSolicitudNavigation.FechaFin > DateOnly.FromDateTime(DateTime.Now)).ToListAsync();

            var repos = _context.JustificacionesInconsistencias
                .Where(r => reposicionesList.Contains(r.IdJustificacion) && r.Reposicion == null)
                .ToList();

            return View("/Views/Paginas/reposiciones/solicitarRepo.cshtml", repos);
        }


        [Authorize(Roles = "COLABORADOR")]
        public IActionResult RegistroActividadesColaborador()
        {
            return RedirectToAction("Create", "RegistroActividades");
        }

        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult RegistroActividadesSupervisor()
        {
            return RedirectToAction("IndexSupervisor", "RegistroActividades");
        }

        [Authorize(Roles = "JEFATURA")]
        public IActionResult RegistroActividadesJefatura()
        {
            return RedirectToAction("ControlGeneral", "RegistroActividades");
        }
        // Para los colaboradores
        [Authorize(Roles = "COLABORADOR")]
        public IActionResult SolicitudPermisoColaborador()
        {
            return View("~/Views/SolicitudPermiso/create.cshtml");
        }

        // Para los supervisores
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult SolicitudPermisoSupervisor()
        {
            return View("~/Views/SolicitudPermiso/ReporteGeneral.cshtml" );
        }

        // Para la jefatura
        [Authorize(Roles = "JEFATURA")]
        public IActionResult SolicitudPermisoJefatura()
        {
            return View("~/Views/SolicitudPermiso/Aprobacion.cshtml");
        }

        [Authorize(Roles = "COLABORADOR")]
        public IActionResult Horasextrascolaborador()
        {
            return View("~/Views/Paginas/Gestion_Horas_Extras/AprobacionHorasExtras.cshtml");
        }

        // Para los supervisores
        [Authorize(Roles = "SUPERVISOR")]
        public IActionResult Horasextrassupervisor()
        {
            return View("~/Views/Paginas/Gestion_Horas_Extras/ReporteHorasExtras.cshtml");
        }

        // Para la jefatura
        [Authorize(Roles = "JEFATURA")]
        public IActionResult HorasextrasJefatura()
        {
            return View("~/Views/Paginas/Gestion_Horas_Extras/SolicitarHorasExtras.cshtml");
        }

    }
}





