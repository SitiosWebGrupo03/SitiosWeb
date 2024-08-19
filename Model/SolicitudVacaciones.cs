using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class SolicitudVacaciones
{
    public int IdSolicitud { get; set; }

    public string? IdEmpleado { get; set; }

    public bool Aprobadas { get; set; }

    public string AprobadasPor { get; set; } = null!;

    public int TotalDias { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual ICollection<Vacaciones> Vacaciones { get; set; } = new List<Vacaciones>();
}
