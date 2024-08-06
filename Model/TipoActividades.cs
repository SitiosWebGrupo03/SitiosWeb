using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class TipoActividades
{
    public int IdTipoActividad { get; set; }

    public string Nombre { get; set; } = null!;

    public string NomActividad { get; set; } = null!;

    public virtual ICollection<HorasExtra> HorasExtra { get; set; } = new List<HorasExtra>();

    public virtual ICollection<RegistroActividades> RegistroActividades { get; set; } = new List<RegistroActividades>();

    public virtual ICollection<SolicitudHorasExtra> SolicitudHorasExtra { get; set; } = new List<SolicitudHorasExtra>();
}
