using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class ReporteMarcas
{
    public int IdReporte { get; set; }

    public string? IdEmpleado { get; set; }

    public int? IdMarca { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual Marcas? IdMarcaNavigation { get; set; }
}
