using EventosApi.Data;

namespace EventosApi.DTOs
{
    public class GetUsuarioDTO
    {
        public int UsuarioId { get; set; }

        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }
    }
}
