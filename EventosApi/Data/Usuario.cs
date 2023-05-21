namespace EventosApi.Data
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        
        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public List<Evento> Favoritos { get; set; } = new List<Evento>();

       // public List<Evento> Historial { get; set; } = new List<Evento>();

        public List<Organizador> seguidos { get; set; } = new List<Organizador>();


    }
}