using System;
using System.Collections.Generic;

namespace SitiosWeb.Model;

public partial class FechasReposicion
{
    public int IdReposicion { get; set; }

    public DateOnly DiasReposicion { get; set; }

    public double HorasReposicion { get; set; }

    public virtual Reposiciones IdReposicionNavigation { get; set; } = null!;
}
