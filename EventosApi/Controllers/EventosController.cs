namespace EventosApi.Controllers{
    using EventosApi.Data;
    using EventosApi.Migrations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;

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
        /*[HttpGet("eventos")]
        public IActionResult ObtenerEventosConOrganizadores()
        {
            var eventos = context.Eventos.Include(a => a.Organizadores).ToList();
            return Ok(eventos);
        }*/

        [HttpGet("Busqueda por Nombre, Fecha, Ubicación o general")]
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

            var eventos = context.Eventos.Include(a => a.Organizadores).ToList();

            if (eventos.Count == 0)
            {
                return NotFound("El evento no fue encontrado");
            }
            return Ok(eventos);
        }

        [HttpGet("EventosPopulares")]
        public IActionResult GetEventosPopulares()
        {
            var eventosPopulares = context.Eventos.OrderByDescending(e => e.Registrados.Count()).Take(3).ToList();

            return Ok(eventosPopulares);
        }

        [HttpPost]
        public async Task<ActionResult<Evento>> Post(Evento evento){

            evento.EspaciosDisponibles = evento.Capacidad_Maxima;
            context.Add(evento);
            await context.SaveChangesAsync();
            return Ok(evento);
        }

        [HttpPost("Agregar Organizador a Evento")]
        public IActionResult AgregarOrganizadorAEvento(int EventoId, int OrganizadorId)
        {
            var evento = context.Eventos.Include(e => e.Organizadores).FirstOrDefault(c => c.EventoId == EventoId);
            var organizador = context.Organizadores.FirstOrDefault(o => o.OrganizadorId == OrganizadorId);

            if (evento == null)
            {
                return NotFound("El evento no existe");
            }

            if (organizador == null)
            {
                return NotFound("El organizador no existe");
            }
            
            if (evento.Organizadores.Any(e => e.OrganizadorId == organizador.OrganizadorId))
            {
                return BadRequest("El organizador ya está asociado al evento");
            }

            evento.Organizadores.Add(organizador);
            context.SaveChanges();
            
            return Ok("Organizador agregado al evento exitosamente");
        }

        [HttpDelete("Eliminar Organizador de Evento")]
        public IActionResult EliminarOrganizadordeEvento(int EventoId,int OrganizadorId)
        {
            var evento = context.Eventos.Include(e => e.Organizadores).FirstOrDefault(c => c.EventoId == EventoId);
            if (evento == null)
            {
                return NotFound("El evento no fue encontrado");
            }
            var organizador = evento.Organizadores.FirstOrDefault(o => o.OrganizadorId == OrganizadorId);
            var orga = context.Organizadores.FirstOrDefault(o => o.OrganizadorId == OrganizadorId);

            if (orga == null)
            {
                return NotFound("El organizador no existe ");
            }

            if (organizador == null)
            {
                return NotFound("El organizador no se encuentra en el evento ");
            }
            

            evento.Organizadores.Remove(organizador);
            context.SaveChanges();
            return Ok("Se elimino el organizador del evento");

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

        [HttpPost("Registro")]
        public IActionResult RegistrarUsuarioEnEvento(int usuarioId, int eventoId, string? CodigoPromocion = null)
        {
            var usuario = context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            var evento = context.Eventos.Include(o=>o.Registrados).FirstOrDefault(e => e.EventoId == eventoId);
            

            if (usuario == null || evento == null)
            {
                return NotFound("El usuario o el evento no existen");
            }

            if (evento.EspaciosDisponibles <= 0)
            {
                return BadRequest("No hay espacios disponibles en el evento");
            }

            if (evento.Registrados.Any(u => u.UsuarioId == usuarioId))
            {
                return BadRequest("El usuario ya está registrado en el evento");
            }

            float costoTotal = evento.Costo;

            if (!string.IsNullOrEmpty(CodigoPromocion))
            {
                var promocion = context.Promociones.FirstOrDefault(p => p.CodigoPromocion == CodigoPromocion);

                if (promocion != null && promocion.EventoId == eventoId)
                {
                    costoTotal = costoTotal*promocion.descuento;
                }
            }

            Asistencia asistencia = new Asistencia();
            asistencia.EventoId = eventoId;
            asistencia.UsuarioId = usuarioId;
            asistencia.AsistenciaEvento = true;
            context.Asistencias.Add(asistencia);

            evento.EspaciosDisponibles--;
            evento.Registrados.Add(usuario);
            context.SaveChanges();

            return Ok($"Registro exitoso. Costo total: {costoTotal}");
        }

        [HttpDelete("CancelarRegistro")]
        public async Task<ActionResult> DeleteRegistro(int UsuarioId, int EventoId)
        {
            
            var evento =  context.Eventos.Include(o => o.Registrados).FirstOrDefault(e => e.EventoId == EventoId);
            if (evento == null)
            {
                return NotFound("El evento no fue encontrado");
            }
            var usuario = context.Usuarios.FirstOrDefault(u => u.UsuarioId == UsuarioId);
            var usuarioEnEvento = evento.Registrados.FirstOrDefault(u => u.UsuarioId == UsuarioId);
            if (usuario == null)
            {
                return NotFound("El usuario no fue encontrado");
            }

            if (usuarioEnEvento == null)
            {
                return BadRequest("El usuario no está registrado en el evento");
            }
            var asistencia = await context.Asistencias.FirstOrDefaultAsync(a => a.EventoId == EventoId && a.UsuarioId == UsuarioId);

            evento.EspaciosDisponibles++;
            context.Asistencias.Remove(asistencia);
            evento.Registrados.Remove(usuarioEnEvento);
            await context.SaveChangesAsync();
            return Ok("Se elimino el registro al evento");
        }
    }
}