using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class VacacionesColectivas
{
    public int IdVacaciones { get; set; }

    public string? IdSolicitador { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Aprobado { get; set; }

    public int? IdDepartamento { get; set; }

    public virtual Departamentos? IdDepartamentoNavigation { get; set; }

    public virtual Colaboradores? IdSolicitadorNavigation { get; set; }
}
