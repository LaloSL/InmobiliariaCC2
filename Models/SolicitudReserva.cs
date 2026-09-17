using System.ComponentModel.DataAnnotations;

namespace InmobiliariaCC2.Models
{
    public class SolicitudReserva
    {
        public int IdSolicitud { get; set; }


        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;


        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellido { get; set; } = string.Empty;


        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20)]
        public string Dni { get; set; } = string.Empty;


        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe seleccionar un inmueble.")]
        public int IdInmueble { get; set; }

        public Inmueble? Inmueble { get; set; }


        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaDesde { get; set; }


        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }


        public DateTime FechaSolicitud { get; set; }


        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";
    }
}