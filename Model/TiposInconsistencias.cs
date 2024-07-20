using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class TiposInconsistencias
{
    public int IdTipoInconsistencia { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<Inconsistencias> Inconsistencias { get; set; } = new List<Inconsistencias>();

    public virtual ICollection<JustificacionesInconsistencias> JustificacionesInconsistencias { get; set; } = new List<JustificacionesInconsistencias>();
}
