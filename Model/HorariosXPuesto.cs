using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class HorariosXPuesto
{
    public int? Idhorario { get; set; }

    public string? IdPuesto { get; set; }

    public string? Lunes { get; set; } 

    public string? Martes { get; set; } 

    public string? Miercoles { get; set; } 
    public string? Jueves { get; set; } 

    public string? Viernes { get; set; } 

    public string? Sabado { get; set; } 

    public bool? Estado { get; set; }

    public virtual Puestos? IdPuestoNavigation { get; set; }
}
