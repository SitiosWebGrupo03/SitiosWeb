using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class HorasExtra
{
    public int IdHoras { get; set; }

    public string? IdEmpleado { get; set; }

    public DateOnly FechaRealizacion { get; set; }

    public int? IdTipoActividad { get; set; }

    public bool EstadoDelPago { get; set; }

    public decimal CantidadHoras { get; set; }

    public string? IdSolicitante { get; set; }

    public decimal CantidadPago { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual Colaboradores? IdSolicitanteNavigation { get; set; }

    public virtual TipoActividades? IdTipoActividadNavigation { get; set; }
}
