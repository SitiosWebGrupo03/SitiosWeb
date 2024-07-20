using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class TipoUsuario
{
    public int IdTipo { get; set; }

    public string NomTipo { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}
