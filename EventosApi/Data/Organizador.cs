namespace EventosApi.Data;

public class Organizador
{
    public int OrganizadorId { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }

    public List<Evento> Eventos { get; set; } = new List<Evento> ();

    public List<Usuario> Seguidores { get; set; } = new List<Usuario> ();
}