using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Marcas
{
    public int IdMarca { get; set; }

    public string? IdEmpleado { get; set; }

    public DateTime InicioJornada { get; set; }

    public DateTime FinJornada { get; set; }

    public virtual Colaboradores? IdEmpleadoNavigation { get; set; }

    public virtual ICollection<ReporteMarcas> ReporteMarcas { get; set; } = new List<ReporteMarcas>();
}
