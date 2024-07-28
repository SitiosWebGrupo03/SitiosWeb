using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Colaboradores
{
    public string Identificacion { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public DateOnly FechaNaciento { get; set; }

    public DateOnly FechaContratacion { get; set; }

    public DateOnly FechaFinContrato { get; set; }

    public string? IdPuesto { get; set; }

    public string? Correo { get; set; }

    public int? Telefono { get; set; }

   
    public virtual ICollection<HorasExtra> HorasExtraIdEmpleadoNavigation { get; set; } = new List<HorasExtra>();

    public virtual ICollection<HorasExtra> HorasExtraIdSolicitanteNavigation { get; set; } = new List<HorasExtra>();

    public virtual Puestos? IdPuestoNavigation { get; set; }

    public virtual ICollection<Inconsistencias> Inconsistencias { get; set; } = new List<Inconsistencias>();

    public virtual ICollection<JustificacionesInconsistencias> JustificacionesInconsistencias { get; set; } = new List<JustificacionesInconsistencias>();

    public virtual ICollection<Marcas> Marcas { get; set; } = new List<Marcas>();

    public virtual ICollection<Permisos> PermisosIdEmpleadoNavigation { get; set; } = new List<Permisos>();

    public virtual ICollection<Permisos> PermisosIdValidadorNavigation { get; set; } = new List<Permisos>();

    public virtual ICollection<Rebajos> RebajosIdColaboradorNavigation { get; set; } = new List<Rebajos>();

    public virtual ICollection<Rebajos> RebajosIdValidadorNavigation { get; set; } = new List<Rebajos>();

    public virtual ICollection<RegistroActividades> RegistroActividadesIdColaboradorNavigation { get; set; } = new List<RegistroActividades>();

    public virtual ICollection<RegistroActividades> RegistroActividadesIdValidadorNavigation { get; set; } = new List<RegistroActividades>();

    public virtual ICollection<ReporteMarcas> ReporteMarcas { get; set; } = new List<ReporteMarcas>();

    public virtual ICollection<ReporteVacaciones> ReporteVacacionesIdEmpleadoNavigation { get; set; } = new List<ReporteVacaciones>();

    public virtual ICollection<ReporteVacaciones> ReporteVacacionesIdValidadorNavigation { get; set; } = new List<ReporteVacaciones>();

    public virtual ICollection<Reposiciones> ReposicionesAprobadasPorNavigation { get; set; } = new List<Reposiciones>();

    public virtual ICollection<Reposiciones> ReposicionesIdcolaboradorNavigation { get; set; } = new List<Reposiciones>();

    public virtual ICollection<SolicitudHorasExtra> SolicitudHorasExtraIdEmpleadoNavigation { get; set; } = new List<SolicitudHorasExtra>();

    public virtual ICollection<SolicitudHorasExtra> SolicitudHorasExtraIdSolicitanteNavigation { get; set; } = new List<SolicitudHorasExtra>();

    public virtual ICollection<SolicitudPermiso> SolicitudPermiso { get; set; } = new List<SolicitudPermiso>();

    public virtual ICollection<SolicitudVacaciones> SolicitudVacaciones { get; set; } = new List<SolicitudVacaciones>();

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();

    public virtual ICollection<Vacaciones> Vacaciones { get; set; } = new List<Vacaciones>();

    public virtual ICollection<VacacionesColectivas> VacacionesColectivas { get; set; } = new List<VacacionesColectivas>();
}
