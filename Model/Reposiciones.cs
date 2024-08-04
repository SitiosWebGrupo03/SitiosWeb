using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Reposiciones
{
    public int IdReposicion { get; set; }

    public string HorasReponer { get; set; } = null!;

    public string Idcolaborador { get; set; } = null!;

    public bool? Apobadas { get; set; }

    public string? AprobadasPor { get; set; }

    public virtual Colaboradores? AprobadasPorNavigation { get; set; }

    public virtual ICollection<FechasReposicion> FechasReposicion { get; set; } = new List<FechasReposicion>();

    public virtual Colaboradores IdcolaboradorNavigation { get; set; } = null!;

    public virtual ICollection<JustificacionesInconsistencias> JustificacionesInconsistencias { get; set; } = new List<JustificacionesInconsistencias>();
}
