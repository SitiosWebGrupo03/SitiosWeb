using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Permisos
{
    public int IdPermiso { get; set; }

    public string? IdEmpleado { get; set; }

    public int? IdTipoPermiso { get; set; }

    public bool DOH { get; set; }

    public int DiasHorasFuera { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string? IdValidador { get; set; }

    public bool Aprobado { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual TiposPermisos? IdTipoPermisoNavigation { get; set; }

    public virtual Colaboradores? IdValidadorNavigation { get; set; }
}
