using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Vacaciones
{
    public int IdSolicitud { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual SolicitudVacaciones IdSolicitudNavigation { get; set; } = null!;
}
