using System.ComponentModel.DataAnnotations;

namespace EventosApi.DTOs
{
    public class EditarAccesoDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
