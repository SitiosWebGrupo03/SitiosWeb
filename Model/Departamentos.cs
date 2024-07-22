using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Departamentos
{
    public int IdDepartamento { get; set; }

    public string NomDepartamento { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<JustificacionesInconsistencias> JustificacionesInconsistencias { get; set; } = new List<JustificacionesInconsistencias>();

    public virtual ICollection<Puestos> Puestos { get; set; } = new List<Puestos>();

    public virtual ICollection<VacacionesColectivas> VacacionesColectivas { get; set; } = new List<VacacionesColectivas>();
}
