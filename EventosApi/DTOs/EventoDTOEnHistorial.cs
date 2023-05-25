namespace EventosApi.DTOs
{
    public class EventoDTOEnHistorial
    {
        public int EventoId { get; set; }
        public string Nombre_Evento { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
        public float Costo { get; set; }
    }
}
