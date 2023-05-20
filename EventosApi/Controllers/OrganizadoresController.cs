namespace EventosApi.Controllers
{
    using EventosApi.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/organizadores")]
    public class OrganizadoresController : ControllerBase{
        private readonly ApplicationDbContext context;
        public OrganizadoresController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Organizador>>> Get()
        {
            return await context.Organizadores.ToListAsync();
        }

        /*[HttpPost]
        public async Task<ActionResult<Organizador>> Post(Organizador organizador)
        {
            context.Add(organizador);
            await context.SaveChangesAsync();
            return Ok(organizador);
        }*/

        [HttpPost]
        public async Task<ActionResult<Organizador>> CrearOrganizador(int EventoId,[FromBody] Organizador Organizador)
        {
            var evento = await context.Eventos.FindAsync(EventoId);

            if (evento == null)
            {
                return NotFound("El evento no existe");
            }

            Organizador.EventoId = EventoId;
            evento.Organizadores.Add(Organizador);
            context.Add(Organizador);

            await context.SaveChangesAsync();

            return Ok(Organizador);
            //return CreatedAtAction("GetOrganizador", new { id = nuevoOrganizador.OrganizadorId }, nuevoOrganizador);
        }
    }
}
