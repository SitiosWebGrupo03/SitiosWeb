namespace SitiosWeb.Model
{
    public class SoliciditudIncapacida
    {
        public int IdSolicitud { get; set; }

        public string? IdEmpleado { get; set; }

        public bool DOH { get; set; }

        public int DiasHorasFuera { get; set; }

        public string Comentarios { get; set; } = null!;

        public int? IdTipoPermiso { get; set; }

        public string? puestoLaboral { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
