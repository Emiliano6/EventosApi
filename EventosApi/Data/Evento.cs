namespace EventosApi.Data;
public class Evento{
    public int EventoId { get; set; }
    public string? Nombre_Evento { get; set; }
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public string? Ubicacion { get; set; }
    public int Capacidad_Maxima { get; set; }
    public List<Organizador> Organizadores { get; set; }
    
}