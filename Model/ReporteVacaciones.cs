using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class ReporteVacaciones
{
    public int IdReporte { get; set; }

    public string? IdEmpleado { get; set; }

    public string? IdValidador { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Estado { get; set; }

    public DateOnly FechaAprobacion { get; set; }

    public string? Observaciones { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual Colaboradores? IdValidadorNavigation { get; set; }
}
