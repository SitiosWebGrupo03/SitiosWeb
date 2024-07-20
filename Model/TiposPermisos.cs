using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class TiposPermisos
{
    public int IdTipoPermiso { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<Permisos> Permisos { get; set; } = new List<Permisos>();

    public virtual ICollection<SolicitudPermiso> SolicitudPermiso { get; set; } = new List<SolicitudPermiso>();
}
