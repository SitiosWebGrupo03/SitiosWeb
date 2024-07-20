using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class SolicitudVacaciones
{
    public int IdSolicitud { get; set; }

    public string? IdEmpleado { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int? DiasTotales { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }
}
