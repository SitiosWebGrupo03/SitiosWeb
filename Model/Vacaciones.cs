using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Vacaciones
{
    public int IdVacaciones { get; set; }

    public string? IdEmpleado { get; set; }

    public int? DiasAcumulados { get; set; }

    public int? AniosEnEmpresa { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }
}
