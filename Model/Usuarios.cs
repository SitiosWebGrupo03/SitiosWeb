using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Usuarios
{
    public string CodUsuario { get; set; } = null!;

    public int? IdTipoUsuario { get; set; }

    public string Contrasena { get; set; } = null!;

    public string? IdColaborador { get; set; }

    public bool Estado { get; set; }

    public virtual Colaboradores? IdColaboradorNavigation { get; set; }

    public virtual TipoUsuario? IdTipoUsuarioNavigation { get; set; }
}
