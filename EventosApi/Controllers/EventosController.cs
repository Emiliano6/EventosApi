namespace EventosApi.Controllers{
    using EventosApi.Data;
    using EventosApi.Migrations;
    using EventosApi.Services;
    using Microsoft.AspNetCore.Connections.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Net;
    using System.Net.Mail;

    [ApiController]
    [Route("api/eventos")]
    public class EventosController : ControllerBase{
        private readonly ApplicationDbContext context;
        public IConfiguration Configuration { get; }
        private SmtpClient smtpClient;
        public EventosController(ApplicationDbContext context, IConfiguration configuration){
            this.context = context;
            Configuration = configuration;
            smtpClient = new SmtpClient("smtp.zoho.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["Email:Address"], Configuration["Email:Password"]),
                EnableSsl = true,
            };
        }


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
            //DateTime hoy = DateTime.Now;
            //DateTime mañana = DateTime.Now.AddDays(1);

            if (eventos.Count == 0)
            {
                return NotFound("El evento no fue encontrado");
            }
            /*foreach (var evento in eventos)
            {
                if (evento.Fecha == mañana)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(Configuration["Email:Address"]),
                        Subject = string.Format("Evento proximo"),
                        Body = string.Format("Hola, el evento {0} es el dia de mañana, te esperamos", evento.Nombre_Evento),
                        IsBodyHtml = false,
                    };
                    var usuarios = context.Eventos.Include(e => e.Registrados).FirstOrDefault(o => o.EventoId == evento.EventoId);
                    foreach (var usuario in evento.Registrados)
                    {
                        mailMessage.To.Add(new MailAddress(usuario.Correo));
                    }
                    smtpClient.Send(mailMessage);
                }
            }*/
            
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

        /*[HttpPost("EnviarCorreoRecordatorio")]
        public async Task<IActionResult> EnviarCorreoRecordatorio(int eventoId, int diasAntes)
        {
            var evento = await context.Eventos
                .Include(e => e.Registrados)
                .FirstOrDefaultAsync(e => e.EventoId == eventoId);
            if (evento == null)
            {
                return NotFound("El evento no existe");
            }
            var fechaInicioEvento = evento.Fecha;
            var fechaRecordatorio = fechaInicioEvento.AddDays(-diasAntes);

            var usuariosRegistrados = evento.Registrados;

            foreach (var usuario in usuariosRegistrados)
            {
                // Aquí puedes personalizar el contenido del correo electrónico con los detalles del evento
                var mensaje = $"Estimado {usuario.Nombre},\n\nTe recordamos que el evento {evento.Nombre_Evento} está programado para el día {fechaInicioEvento.ToShortDateString()}. No olvides asistir y disfrutar del evento.\n\nSaludos,\nEquipo de EventosApi";

                await emailService.SendEmailAsync(usuario.Correo, "Recordatorio de Evento", mensaje);
            }
            return Ok("Correos de recordatorio enviados correctamente");
        }*/
    }
}