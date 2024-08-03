using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class SolicitudHorasExtra
{
    public int IdSolicitud { get; set; }

    public string? IdSolicitante { get; set; }

    public string? IdEmpleado { get; set; }

    public DateOnly FechaSolicitud { get; set; }

    public decimal Horas { get; set; }

    public int? IdTipoActividad { get; set; }

    public string Estado { get; set; } = null!;

    public string? AprobadaPor { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual Colaboradores? IdSolicitanteNavigation { get; set; }

    public virtual TipoActividades? IdTipoActividadNavigation { get; set; }
}
