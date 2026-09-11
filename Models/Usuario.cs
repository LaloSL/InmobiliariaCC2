using System.ComponentModel.DataAnnotations;

namespace InmobiliariaCC2.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$",
            ErrorMessage = "El nombre solo puede contener letras")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un email válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string PasswordHash { get; set; } = "";

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; } = "";

        public bool Estado { get; set; } = true;
    }
}