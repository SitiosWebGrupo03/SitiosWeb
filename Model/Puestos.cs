using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Puestos
{
    public string IdPuesto { get; set; } = null!;

    public string NombrePuesto { get; set; } = null!;

    public decimal Salario { get; set; }

    public int? IdDepartamento { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Colaboradores> Colaboradores { get; set; } = new List<Colaboradores>();

    public virtual ICollection<HorariosXPuesto> HorariosXPuesto { get; set; } = new List<HorariosXPuesto>();

    public virtual Departamentos? IdDepartamentoNavigation { get; set; }

    public virtual ICollection<JustificacionesInconsistencias> JustificacionesInconsistencias { get; set; } = new List<JustificacionesInconsistencias>();
}
