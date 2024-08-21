

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

        [HttpPost]
        public async Task<IActionResult> GestionAprobr(string identificacion, string diasFuera, DateTime[] FechaInicio, DateTime[] FechaFin, string puestoLaboral, string IdTipoPermiso, int estado)
        {
            // Convertir los valores separados por comas en listas
            var ids = identificacion?.Split(',') ?? Array.Empty<string>();
            var diasHoras = diasFuera?.Split(',') ?? Array.Empty<string>();
            var puestos = puestoLaboral?.Split(',') ?? Array.Empty<string>();
            var tiposPermiso = IdTipoPermiso?.Split(',') ?? Array.Empty<string>();

            // Convertir fechas desde los arrays de strings a DateTime
            var fechasInicio = FechaInicio;
            var fechasFin = FechaFin;

            // Validar los datos recibidos
            if (!ids.Any() || ids.Length != diasHoras.Length || ids.Length != puestos.Length || ids.Length != tiposPermiso.Length || ids.Length != fechasInicio.Length || ids.Length != fechasFin.Length)
            {
                TempData["ErrorMessage"] = "Datos incompletos o incorrectos.";
                return View("~/Views/Incapacidades/AprobacionInco.cshtml", await _context.SolicidtudIncapadad.AsNoTracking().ToListAsync());
            }

            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC ProcesarSolicitudPermiso @estado, @IdEmpleado, @DOH, @DiasHorasFuera, @IdTipoPermiso, @PuestoLaboral, @FechaInicio, @FechaFin",
                        new SqlParameter("@estado", estado),
                        new SqlParameter("@IdEmpleado", ids[i]),
                        new SqlParameter("@DOH", true),
                        new SqlParameter("@DiasHorasFuera", int.Parse(diasHoras[i])),
                        new SqlParameter("@IdTipoPermiso", tiposPermiso[i]),
                        new SqlParameter("@PuestoLaboral", puestos[i]),
                        new SqlParameter("@FechaInicio", fechasInicio[i]),
                        new SqlParameter("@FechaFin", fechasFin[i])
                    );
                }

                TempData["SuccessMessage"] = "Las solicitudes han sido procesadas con Ã©xito.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Se produjo un error al procesar las solicitudes: {ex.Message}";
            }

            return View("~/Views/Incapacidades/AprobacionInco.cshtml", await _context.SolicidtudIncapadad.AsNoTracking().ToListAsync());
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
