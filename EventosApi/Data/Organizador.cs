namespace EventosApi.Data;

public class Organizador{
    public int OrganizadorId { get; set; }
    public string? Nombre { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
}