using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class ReposicionTercero
{
    public string Idtercero { get; set; } = null!;

    public string Idsolicitante { get; set; } = null!;

    public int Justificacion { get; set; }

    public bool? Aceptado { get; set; }

    public int Horas { get; set; }

    public virtual Colaboradores IdsolicitanteNavigation { get; set; } = null!;

    public virtual Colaboradores IdterceroNavigation { get; set; } = null!;

    public virtual JustificacionesInconsistencias JustificacionNavigation { get; set; } = null!;
}
