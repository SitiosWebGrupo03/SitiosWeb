using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class BloqueoDias
{
    public int DayId { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly Day { get; set; }

    public int Tipo { get; set; }
}
