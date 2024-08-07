using System;
using System.Collections.Generic;

namespace SitiosWeb.Model
{

    public partial class RegistroActividades
    {
        public int IdRegistro { get; set; }


        public int? IdTipoActividad { get; set; }

        public string? IdColaborador { get; set; }

        public string? IdValidador { get; set; }

        public string Observaciones { get; set; } = null!;

        public DateOnly FechaActividad { get; set; }

        public decimal DuracionEnHoras { get; set; }

        public bool Aprobado { get; set; }

        public virtual Colaboradores? IdColaboradorNavigation { get; set; }

        public virtual TipoActividades? IdTipoActividadNavigation { get; set; }

        public virtual Colaboradores? IdValidadorNavigation { get; set; }
    }
}
