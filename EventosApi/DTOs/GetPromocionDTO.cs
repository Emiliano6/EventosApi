namespace EventosApi.DTOs
{
    public class GetPromocionDTO
    {
        public int PromocionId { get; set; }

        public string CodigoPromocion { get; set; }
        public float descuento { get; set; }

        public int EventoId { get; set; }
    }
}
