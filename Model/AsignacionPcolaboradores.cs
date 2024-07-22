using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class AsignacionPcolaboradores
{
    public string Identificacion { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Departamento { get; set; } = null!;

    public string Puesto { get; set; } = null!;
}
