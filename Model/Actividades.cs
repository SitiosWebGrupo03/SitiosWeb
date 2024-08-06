using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Actividades
{
    public int ActividadId { get; set; }

    public string? Identificacion { get; set; }

    public DateTime? FechaActividad { get; set; }

    public string? Descripcion { get; set; }

    public string? TipoActividad { get; set; }

    public decimal? Horas { get; set; }

    public string? Estado { get; set; }

    public string? Comentarios { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaAprobacion { get; set; }

    public virtual Colaboradores? IdentificacionNavigation { get; set; }
}
