using System.ComponentModel.DataAnnotations;

namespace InmobiliariaCC2.Models
{
    public class Reserva
    {
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino.")]
        public int IdInquilino { get; set; }

        public Inquilino? Inquilino { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inmueble.")]
        public int IdInmueble { get; set; }

        public Inmueble? Inmueble { get; set; }

        [Required]
        public decimal MontoDia { get; set; }

        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }

        public DateTime? FechaTerminacionAnticipada { get; set; }

        public decimal Multa { get; set; }

        public bool Estado { get; set; } = true;
    }
}