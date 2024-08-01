using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class HorariosXPuesto
{
    public int Idhorario { get; set; }

    public string? IdPuesto { get; set; }

    public string Lunes { get; set; } = null!;

    public string Martes { get; set; } = null!;

    public string Miercoles { get; set; } = null!;

    public string Jueves { get; set; } = null!;

    public string Viernes { get; set; } = null!;

    public string Sabado { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual Puestos? IdPuestoNavigation { get; set; }
}
