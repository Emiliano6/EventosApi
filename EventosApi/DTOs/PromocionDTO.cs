using System.ComponentModel.DataAnnotations;

namespace EventosApi.DTOs
{
    public class PromocionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string CodigoPromocion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public float descuento { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int EventoId { get; set; }
    }
}
