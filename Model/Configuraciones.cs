using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class Configuraciones
{
    public int IdConfiguraciones { get; set; }

    public string NomConfiguracion { get; set; } = null!;

    public string ValorConfig { get; set; } = null!;

    public int? NumConfig { get; set; }
}
