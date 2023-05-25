using AutoMapper;
using EventosApi.Data;
using EventosApi.DTOs;
using EventosApi.Migrations;
using EventosApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Mail;

namespace EventosApi.Controllers{
    [ApiController]
    [Route("api/eventos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class EventosController : ControllerBase{
        private readonly ApplicationDbContext context;
        public IConfiguration Configuration { get; }
        private SmtpClient smtpClient;
        private readonly IMapper mapper;
        public EventosController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper){
            this.context = context;
            this.mapper = mapper;
            Configuration = configuration;
            smtpClient = new SmtpClient("smtp.zoho.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["Email:Address"], Configuration["Email:Password"]),
                EnableSsl = true,
            };
        }
        [AllowAnonymous]
        [HttpGet("Busqueda por Nombre, Fecha, Ubicación o general")]
        public async Task<ActionResult<IEnumerable<GetEventoDTOUsuario>>> Get([FromQuery] string? Nombre_Evento, [FromQuery] DateTime? Fecha, [FromQuery] string? Ubicacion)
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

            var eventos = await query.Include(a => a.Organizadores).ToListAsync();

            if (eventos.Count == 0)
            {
                return NotFound("El evento no fue encontrado");
            }

            var eventosDTO = mapper.Map<List<GetEventoDTOUsuario>>(eventos);

            return eventosDTO;
        }
        [AllowAnonymous]
        [HttpGet("EventosPopulares")]
        public IActionResult GetEventosPopulares()
        {
            var eventosPopulares = context.Eventos.OrderByDescending(e => e.Registrados.Count()).Take(3).ToList();

            var eventosDTO = mapper.Map<List<GetEventoDTOUsuario>>(eventosPopulares);
            return Ok(eventosDTO);
        }

        [HttpPost("Agregar Evento")]
        public async Task<ActionResult<EventoDTO>> Post(EventoDTO eventoDTO){

            
            var evento = mapper.Map<Evento>(eventoDTO);
            context.Add(evento);
            await context.SaveChangesAsync();
            return Ok(evento);
        }

        [HttpPost("Agregar Organizador a Evento")]
        public IActionResult AgregarOrganizadorAEvento(int EventoId, int OrganizadorId)
        {
            var evento = context.Eventos.Include(e => e.Organizadores).FirstOrDefault(c => c.EventoId == EventoId);
            var organizador = context.Organizadores.Include(e => e.Seguidores).FirstOrDefault(o => o.OrganizadorId == OrganizadorId);
            
            if (evento == null)
            {
                return NotFound("El evento no existe");
            }

            if (organizador == null)
            {
                return NotFound("El organizador no existe");
            }
            var usuarios = organizador.Seguidores.ToList();
            if (evento.Organizadores.Any(e => e.OrganizadorId == organizador.OrganizadorId))
            {
                return BadRequest("El organizador ya está asociado al evento");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(Configuration["Email:Address"]),
                Subject = string.Format("Nuevo evento"),
                Body = string.Format("Hola, el organizador {0} al que sigues acaba de crear un nuevo evento.\n Ingresa a nuestra pagina para más información", organizador.Nombre),
                IsBodyHtml = false,
            };
            foreach (var usuario in usuarios)
            {
                mailMessage.To.Add(new MailAddress(usuario.Correo));
            }
            smtpClient.Send(mailMessage);
            evento.Organizadores.Add(organizador);
            context.SaveChanges();
            
            return Ok("Organizador agregado al evento exitosamente");
        }

        [HttpPost("Registrarse a un evento")]
        public IActionResult RegistrarUsuarioEnEvento(int usuarioId, int eventoId, string? CodigoPromocion = null)
        {
            var usuario = context.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioId);
            var evento = context.Eventos.Include(o => o.Registrados).FirstOrDefault(e => e.EventoId == eventoId);


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
                    costoTotal = costoTotal * promocion.descuento;
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

        [HttpPost("/Enviar preguntas o sugerencias")]
        public IActionResult EnviarFormulario(int EventoId, string nombre, string correo, string mensaje)
        {
            // Construir el mensaje de correo
            var evento = context.Eventos.Include(o => o.Organizadores).FirstOrDefault(e => e.EventoId == EventoId);
            if (evento == null)
            {
                return NotFound("El evento al cual quieres enviar comentarios o sugerencias no existe"); // Manejar el caso si el evento no existe
            }

            if (!evento.Organizadores.Any())
            {
                return NotFound("Parece que aun no hay organizadores para ese evento");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(Configuration["Email:Address"]),
                Subject = "Dudas o sugerencias",
                Body = $"Nombre: {nombre}\nCorreo electrónico: {correo}\nMensaje: {mensaje}\nEvento relacionado: {evento.Nombre_Evento}",
                IsBodyHtml = false
            };

            foreach (var organizador in evento.Organizadores)
            {
                mailMessage.To.Add(new MailAddress(organizador.Correo));
            }

            // Enviar el correo electrónico
            smtpClient.Send(mailMessage);

            return Ok("Formulario Enviado");
        }

        [HttpPut("Editar evento")]
        public async Task<ActionResult> Put(EventoDTO EventoDTO, int EventoId)
        {
            var exists = await context.Eventos.AnyAsync(x => x.EventoId == EventoId);
            if (!exists)
            {
                return NotFound("El evento no fue encontrado");
            }
            var evento = mapper.Map<Evento>(EventoDTO);
            evento.EventoId = EventoId;

            context.Update(evento);
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpDelete("Eliminar evento")]
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

        [HttpDelete("Cancelar registro a evento")]
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