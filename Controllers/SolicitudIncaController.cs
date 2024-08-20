

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;
using SitiosWeb.ServicesClass;
using System.Data;

namespace SitiosWeb.Controllers
{
    public class SolicitudIncaController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public SolicitudIncaController(Tiusr22plProyectoContext context)
        {
            _context = context;
           
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerImpactoMonetario(string identificacion)
        {
            if (string.IsNullOrEmpty(identificacion))
            {
                TempData["ErrorMessage"] = "Debe proporcionar un IdEmpleado.";
                return View("~/Views/Incapacidades/ImpactoMometario.cshtml");
            }

            try
            {
                // Parametrización y llamada al procedimiento almacenado
                var nombreColaboradorParam = new SqlParameter("@NombreColaborador", SqlDbType.NVarChar, 300)
                {
                    Direction = ParameterDirection.Output
                };
                var salarioDiarioParam = new SqlParameter("@SalarioDiario", SqlDbType.Decimal)
                {
                    Precision = 10,
                    Scale = 2,
                    Direction = ParameterDirection.Output
                };
                var diasIncapacidadParam = new SqlParameter("@DiasIncapacidad", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Grupo03.CalcularImpactoMonetario @IdEmpleado, @NombreColaborador OUTPUT, @SalarioDiario OUTPUT, @DiasIncapacidad OUTPUT",
                    new SqlParameter("@IdEmpleado", identificacion),
                    nombreColaboradorParam,
                    salarioDiarioParam,
                    diasIncapacidadParam
                );

                var pagoPatrono = 0.0;
                var nombreColaborador = nombreColaboradorParam.Value as string;
                var salarioDiario = (decimal)(salarioDiarioParam.Value ?? 0);
                var diasIncapacidad = (int)(diasIncapacidadParam.Value ?? 0);
                var pagoCaja = 0.0;
                var pagoDCaja = 0.0;
                if (salarioDiario == 0 && nombreColaborador == null && diasIncapacidad == 0)
                {
                    TempData["ErrorMessage"] = "No Existen Datos para esa Identificacion.";
                    return View("~/Views/Incapacidades/ImpactoMometario.cshtml");

                }
                else if (diasIncapacidad <= 3)
                {
                    var pagoDiarioPatrono = salarioDiario / 30;
                    pagoPatrono = ((int)(diasIncapacidad * pagoDiarioPatrono / 100) * 50);
                }
                else if (diasIncapacidad > 3)
                {
                    var pagoDiarioCaja = salarioDiario / 30;
                    pagoDCaja = ((int)(diasIncapacidad * pagoDiarioCaja / 100) * 60);
                }

                ViewBag.Resultado = new ImpactoMonetarioResult
                {
                    NombreColaborador = nombreColaborador,
                    SalarioDiario = salarioDiario,
                    DiasIncapacidad = (int?)pagoPatrono,
                    PagoDLCaja = pagoDCaja
                };
                return View("~/Views/Incapacidades/ImpactoMometario.cshtml");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al obtener el impacto monetario: " + ex.Message;
                return View("~/Views/Incapacidades/ImpactoMometario.cshtml");
            }
        }


        [HttpGet]
        public async Task<IActionResult> CargarTAblaCg()
        {
            var permisos = await _context.SolicidtudIncapadad.AsNoTracking().ToListAsync();


            return View("~/Views/Incapacidades/ControlGenrenalInc.cshtml", permisos);

        }



        [HttpGet]
        public async Task<IActionResult> cargarTablaIncapacidades()
        {
            var permisos = await _context.SolicidtudIncapadad.AsNoTracking().ToListAsync();


            return View("~/Views/Incapacidades/AprobacionInco.cshtml", permisos);
        }
        [HttpPost("Aprob")]
        public async Task<IActionResult> GestionAprob([FromBody] List<Incapacidades> permisos)
        {
            if (permisos == null || !permisos.Any())
            {
                TempData["ErrorMessage"] = "No se recibieron permisos.";
                return View("~/Views/Incapacidades/AprobacionoDeneInca.cshtml", await _context.SolicitudPermiso.AsNoTracking().ToListAsync());
            }

            try
            {
                foreach (var permiso in permisos)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC ProcesarSolicitudPermiso @estado, @IdEmpleado, @DOH, @DiasHorasFuera, @IdTipoPermiso, @PuestoLaboral, @FechaInicio, @FechaFin",
                        new SqlParameter("@estado", permiso.Estado),
                        new SqlParameter("@IdEmpleado", permiso.IdEmpleado),
                        new SqlParameter("@DOH", true),
                        new SqlParameter("@DiasHorasFuera", permiso.DiasHorasFuera),
                        new SqlParameter("@IdTipoPermiso", permiso.IdTipoPermiso),
                        new SqlParameter("@PuestoLaboral", permiso.puestoLaboral),
                        new SqlParameter("@FechaInicio", permiso.FechaInicio),
                        new SqlParameter("@FechaFin", permiso.FechaFin)
                    );
                }

                TempData["SuccessMessage"] = "Puesto asignado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al realizar su transacción: " + ex.Message;
            }


            var permisosActualizados = await _context.SolicidtudIncapadad.AsNoTracking().ToListAsync();
            return View("~/Views/Incapacidades/AprobacionoDeneInca.cshtml", permisosActualizados);
        }

        [HttpPost]
        public IActionResult SolicitudIncapacidad(string identificacion, string puestoLaboral, DateTime FechaInicio, DateTime FechaFin, string idTipoPermiso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuario = _context.Colaboradores.FirstOrDefault(u => u.Identificacion == identificacion);

                    if (usuario == null)
                    {
                        TempData["ErrorMessage"] = "El usuario con esa identificación no existe.";
                        ViewBag.Permisos = _context.TiposPermisos.ToList();
                        return View("~/Views/Incapacidades/SolicitudIncapacidades.cshtml");
                    }


                    int cantidadDias = (FechaFin - FechaInicio).Days;

                    var nuevaSolicitud = new SoliciditudIncapacida
                    {
                        IdEmpleado = identificacion,
                        puestoLaboral = puestoLaboral,
                        FechaInicio = FechaInicio,
                        FechaFin = FechaFin,
                        IdTipoPermiso = Convert.ToInt32(idTipoPermiso),
                        Comentarios = "",
                        DiasHorasFuera = cantidadDias
                    };

                    _context.SolicidtudIncapadad.Add(nuevaSolicitud);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Su solicitud ha sido enviada con éxito.";
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "Ocurrió un error al procesar su solicitud. Por favor, intente nuevamente más tarde.";
            }

            ViewBag.Permisos = _context.TiposPermisos.ToList();
            return View("~/Views/Incapacidades/SolicitudIncapacidades.cshtml");
        }




        [HttpGet]
        public async Task<IActionResult> cargarCBXpermisos()
        {
            try
            {
                var permisos = await _context.TiposInc
                    .Where(tp => tp.Estado)
                    .Select(tp => new
                    {
                        tp.IdTipoPermiso,
                        tp.Descripcion
                    })
                    .ToListAsync();

                // Asegúrate de esperar la tarea antes de asignarla a ViewBag
                ViewBag.Permisos = permisos;

                return View("~/Views/Incapacidades/SolicitudIncapacidades.cshtml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error: " + ex.Message });
            }
        }


    }
}
