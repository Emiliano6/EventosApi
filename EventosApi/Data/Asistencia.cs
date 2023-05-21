namespace EventosApi.Data
{
    public class Asistencia
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int EventoId { get; set; }

        public bool AsistenciaEvento { get; set; }
    }
}
