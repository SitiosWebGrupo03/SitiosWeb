using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class SolicitudVacaciones
{
    public int IdSolicitud { get; set; }

    public string IdEmpleado { get; set; } = null!;

    public bool? Aprobadas { get; set; }

    public string? AprobadasPor { get; set; }

    public int TotalDias { get; set; }

    public DateOnly FechaFin { get; set; }

    public virtual Colaboradores IdEmpleadoNavigation { get; set; } = null!;

    public virtual ICollection<Vacaciones> Vacaciones { get; set; } = new List<Vacaciones>();
}
