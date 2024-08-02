using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class JustificacionesInconsistencias
{
    public int IdJustificacion { get; set; }

    public string? IdColaborador { get; set; }

    public string? IdPuesto { get; set; }

    public int? IdDepartamento { get; set; }

    public int? IdTipoInconsistencia { get; set; }

    public bool? ReponeTiempo { get; set; }

    public string? HorarioId { get; set; }

    public DateOnly FechaInconsistencia { get; set; }

    public string? Observaciones { get; set; }

    public int? Reposicion { get; set; }

    public byte[]? Evidencias { get; set; }

    public virtual Colaboradores? IdColaboradorNavigation { get; set; }

    public virtual Departamentos? IdDepartamentoNavigation { get; set; }

    public virtual Puestos? IdPuestoNavigation { get; set; }

    public virtual TiposInconsistencias? IdTipoInconsistenciaNavigation { get; set; }

    public virtual ICollection<Inconsistencias> Inconsistencias { get; set; } = new List<Inconsistencias>();

    public virtual Reposiciones? ReposicionNavigation { get; set; }
}
