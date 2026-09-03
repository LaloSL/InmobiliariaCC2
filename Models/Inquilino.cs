using System.ComponentModel.DataAnnotations;

namespace InmobiliariaCC2.Models
{
    public class Inquilino
    {
        public int IdInquilino { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20)]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(150)]
        public string? DireccionOrigen { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
    }
}