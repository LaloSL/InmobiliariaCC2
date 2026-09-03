using System.ComponentModel.DataAnnotations;

namespace InmobiliariaCC2.Models
{
    public class Inmueble
    {
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string Direccion { get; set; } = string.Empty;

        [Range(1, 50, ErrorMessage = "El cupo debe ser al menos 1 persona.")]
        public int Cupo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble.")]
        public int IdTipo { get; set; }

        public TipoInmueble? Tipo { get; set; }

        public string? Coordenadas { get; set; }

        [Range(0.01, 1000000.00, ErrorMessage = "Ingrese un precio válido por día.")]
        public decimal PrecioDia { get; set; }

        [Required(ErrorMessage = "Debe asignar un propietario.")]
        public int IdPropietario { get; set; }

        public Propietario? Propietario { get; set; }

        public bool Estado { get; set; } = true;
    }
}