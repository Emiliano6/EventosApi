using System.ComponentModel.DataAnnotations;

namespace EventosApi.DTOs
{
    public class CredencialesUsuario
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Contraseña { get; set; }
    }
}
