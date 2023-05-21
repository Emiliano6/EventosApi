using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventosApi.Data
{
    public class Promocion
    {
        public int PromocionId { get; set; }

        public string CodigoPromocion { get; set; }
        public float descuento { get; set; }

        public int EventoId { get; set; }
    }
}
