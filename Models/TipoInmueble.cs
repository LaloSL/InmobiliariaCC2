using System.ComponentModel.DataAnnotations;

namespace InmobiliariaCC2.Models
{
    public class TipoInmueble
    {
        public int IdTipo { get; set; }

        [Required(ErrorMessage = "El nombre del tipo es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }
}