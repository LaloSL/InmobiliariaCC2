using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public decimal MontoDia { get; set; }

        public decimal PorcentajeReserva { get; set; }


        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaDesde { get; set; }


        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }


        public DateTime? FechaTerminacionAnticipada { get; set; }

        [NotMapped]
        public int CantidadDias
        {
            get
            {
                if (FechaHasta <= FechaDesde)
                    return 0;

                return (FechaHasta - FechaDesde).Days;
            }
        }



        [NotMapped]
        public decimal MontoTotal
        {
            get
            {
                return CantidadDias * MontoDia;
            }
        }

        [NotMapped]
        public decimal MontoMinimoReserva
        {
            get
            {
                return MontoTotal * PorcentajeReserva / 100m;
            }
        }

        public decimal Multa { get; set; }

        public int? IdUsuarioCreacion { get; set; }

        public int? IdUsuarioTerminacion { get; set; }


        public bool Estado { get; set; } = true;
    }
}