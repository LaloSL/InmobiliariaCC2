using System.ComponentModel.DataAnnotations;

namespace InmobiliariaCC2.Models
{
    public class Pago
    {
        public int IdPago { get; set; }


        [Required(ErrorMessage = "La reserva es obligatoria.")]
        public int IdReserva { get; set; }

        public Reserva? Reserva { get; set; }

        [Range(0.01, 100000000,
            ErrorMessage = "El monto del pago debe ser mayor a cero.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha del pago es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debe seleccionar un medio de pago.")]
        [StringLength(50)]
        public string MedioPago { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Observacion { get; set; }

        public bool Estado { get; set; } = true;
    }
}