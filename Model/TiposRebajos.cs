using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class TiposRebajos
{
    public int IdTipoRebajo { get; set; }

    public string Descripcion { get; set; } = null!;

    public int Cantidad { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Rebajos> Rebajos { get; set; } = new List<Rebajos>();
}
