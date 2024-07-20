using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Rebajos
{
    public int IdRebajo { get; set; }

    public string? IdColaborador { get; set; }

    public string? IdValidador { get; set; }

    public DateOnly FechaRebajo { get; set; }

    public int? Inconsistencia { get; set; }

    public int? IdTipoRebajo { get; set; }

    public bool Aprobacion { get; set; }

    public virtual Colaboradores? IdColaboradorNavigation { get; set; }

    public virtual TiposRebajos? IdTipoRebajoNavigation { get; set; }

    public virtual Colaboradores? IdValidadorNavigation { get; set; }

    public virtual Inconsistencias? InconsistenciaNavigation { get; set; }
}
