using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Inconsistencias
{
    public int IdInconsistencia { get; set; }

    public string? IdEmpleado { get; set; }

    public int? IdTipoInconsistencia { get; set; }

    public int? IdJustificacion { get; set; }

    public DateOnly FechaInconsistencia { get; set; }

    public bool? Mostrar { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual JustificacionesInconsistencias? IdJustificacionNavigation { get; set; }

    public virtual TiposInconsistencias? IdTipoInconsistenciaNavigation { get; set; }

    public virtual ICollection<Rebajos> Rebajos { get; set; } = new List<Rebajos>();
}
