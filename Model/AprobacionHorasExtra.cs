using System;
using System.ComponentModel.DataAnnotations;

namespace SitiosWeb.Model
{
    public class AprobacionHorasExtras
    {
        [Required]
        public int Id { get; set; } // Identificador de la solicitud

        [Required]
        public int IdSolicitante { get; set; } // ID del solicitante

        [Required]
        public int IdEmpleado { get; set; } // ID del empleado

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; } // Fecha de la solicitud

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Las horas deben ser mayores a cero.")]
        public decimal Horas { get; set; } // Horas extras solicitadas

        [Required]
        public int IdTipoActividad { get; set; } // Tipo de actividad

        public string Estado { get; set; } // Estado de la solicitud (Pendiente, Aprobado, Denegado)

        // Opcional: Propiedades adicionales si es necesario
        public string Comentarios { get; set; } // Comentarios sobre la solicitud (opcional)
    }
}
