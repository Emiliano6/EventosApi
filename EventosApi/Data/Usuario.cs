namespace EventosApi.Data
{
    public class Usuario
    {
        public int Id { get; set; }
        
        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public List<Evento> Favoritos { get; set; }

        public List<Evento> Historial { get; set; }

        public List<Organizador> seguidos { get; set; }


    }
}