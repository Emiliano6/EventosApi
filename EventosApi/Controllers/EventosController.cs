namespace EventosApi.Controllers{
    using EventosApi.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/eventos")]
    public class EventosController : ControllerBase{
        private readonly ApplicationDbContext context;
        public EventosController(ApplicationDbContext context){
            this.context = context;
        }
       /* [HttpGet]
        public async Task<ActionResult<List<Evento>>> Get()
        {
            return await context.Eventos.ToListAsync();
        }*/

        /*[HttpGet("{Nombre_Evento}")]
        public async Task<ActionResult<Evento>> Get(string Nombre_Evento) 
            {
                var evento = await context.Eventos.FirstOrDefaultAsync(x => x.Nombre_Evento.Contains(Nombre_Evento));

                if (evento == null)
                {
                    return NotFound();
                }
            return evento;
            }*/

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evento>>> Get([FromQuery] string? Nombre_Evento, [FromQuery] DateTime? Fecha, [FromQuery] string? Ubicacion)
        {
            IQueryable<Evento> query = context.Eventos;

            if (!string.IsNullOrEmpty(Nombre_Evento))
            {
                query = query.Where(x => x.Nombre_Evento.Contains(Nombre_Evento));
            }
            if (Fecha.HasValue)
            {
                query = query.Where(x => x.Fecha.Date == Fecha.Value.Date);
            }
            if (!string.IsNullOrEmpty(Ubicacion))
            {
                query = query.Where(x => x.Ubicacion.Contains(Ubicacion));
            }

            var eventos = await query.ToListAsync();

            if (eventos.Count == 0)
            {
                return NotFound("El evento no fue encontrado");
            }
            return eventos;
        }

        [HttpPost]
        public async Task<ActionResult<Evento>> Post(Evento evento){
            context.Add(evento);
            await context.SaveChangesAsync();
            return Ok(evento);
        }

        [HttpPut("{EventoId:int}")]
        public async Task<ActionResult> Put(Evento Evento, int EventoId)
        {
            var exists = await context.Eventos.AnyAsync(x => x.EventoId == EventoId);
            if (!exists)
            {
                return NotFound("El evento no fue encontrado");
            }
            if(Evento.EventoId != EventoId)
            {
                return BadRequest("El id del evento no coincide con el de la url");
            }    
            context.Update(Evento);
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpDelete("{EventoId:int}")]
        public async Task<ActionResult> Delete(int EventoId)
        {
            var exists = await context.Eventos.AnyAsync(x => x.EventoId == EventoId);
            if (!exists)
            {
                return NotFound("El evento no fue encontrado");
            }

            context.Remove(new Evento { EventoId = EventoId });
            await context.SaveChangesAsync();
            return Ok();

        }

    }
}