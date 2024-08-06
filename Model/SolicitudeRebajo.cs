using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class SolicitudeRebajo
{
    public int IdSolicitud { get; set; }

    public string? IdSolicitante { get; set; }

    public int? IdInconsistencia { get; set; }

    public string? Observaciones { get; set; }

    public bool? Mostrar { get; set; }
}
