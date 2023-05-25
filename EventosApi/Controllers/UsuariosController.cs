using System;

namespace EventosApi.Controllers
{
    using AutoMapper;
    using EventosApi.Data;
    using EventosApi.DTOs;
    using EventosApi.Migrations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/Usuario")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public UsuariosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetUsuarioDTO>>> Get()
        {
            var usuarios = context.Usuarios.ToList();
            var usuarioDTO = mapper.Map<List<GetUsuarioDTO>>(usuarios);
            return Ok(usuarioDTO);
        }

        [HttpGet("Eventos favoritos")]
        public async Task<ActionResult<List<EventoDTOEnOrganizador>>> GetFavoritos(int UsuarioId)
        {
            var usuario = await context.Usuarios.Include(u => u.Favoritos).FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);

            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }
            var favoritosDTO = mapper.Map<List<EventoDTOEnOrganizador>>(usuario.Favoritos);

            return Ok(favoritosDTO);
        }

        [HttpGet("Organizadores seguidos")]
        public async Task<ActionResult<List<OrganizadorDTOEnEvento>>> GetSeguidos(int UsuarioId)
        {
            var usuario = await context.Usuarios.Include(u => u.seguidos).FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);

            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }
            var seguidosDTO = mapper.Map<List<OrganizadorDTOEnEvento>>(usuario.seguidos);

            return Ok(seguidosDTO);
        }

        [HttpGet("Historial de eventos")]
        public async Task<ActionResult<List<EventoDTOEnHistorial>>> GetHistorial(int UsuarioId)
        {
            var usuario = await context.Usuarios.Include(u => u.Historial).FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);

            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }
            var eventosHistoricos = usuario.Historial.Select(e => new
            {
                Evento = e,
                Asistencia = context.Asistencias.FirstOrDefault(a => a.UsuarioId == UsuarioId && a.EventoId == e.EventoId)
            }).ToList();

            var resultado = eventosHistoricos.Select(e => new
            {
                e.Evento.EventoId,
                e.Evento.Nombre_Evento,
                e.Evento.Descripcion,
                e.Evento.Fecha,
                e.Evento.Ubicacion,
                e.Evento.Costo,
                Asistencia = e.Asistencia != null ? e.Asistencia.AsistenciaEvento : false
            }).ToList();

            return Ok(resultado);
        }

        [HttpPost("Agregar usuario")]
        public async Task<ActionResult<UsuarioDTO>> Post(UsuarioDTO usuarioDTO)
        {
            var usuario = mapper.Map<Usuario>(usuarioDTO);
            context.Add(usuario);
            await context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPost("Agregar evento a favoritos")]
        public IActionResult AgregarFavorito(int UsuarioId, int EventoId)
        {
            var usuario = context.Usuarios.Include(o => o.Favoritos).FirstOrDefault(o => o.UsuarioId == UsuarioId);
            var evento = context.Eventos.FirstOrDefault(e => e.EventoId == EventoId);

            if (evento == null)
            {
                return NotFound("El evento no existe");
            }

            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }

            if (usuario.Favoritos.Any(e => e.EventoId == evento.EventoId))
            {
                return BadRequest("El evento ya esta agregado como favorito");
            }

            usuario.Favoritos.Add(evento);
            context.SaveChanges();

            return Ok("Evento agregado a favoritos exitosamente");
        }

        [HttpPost("Seguir organizador")]
        public IActionResult SeguirOrganizador(int UsuarioId, int OrganizadorId)
        {
            var usuario = context.Usuarios.Include(o => o.seguidos).FirstOrDefault(o => o.UsuarioId == UsuarioId);
            var organizador = context.Organizadores.FirstOrDefault(e => e.OrganizadorId == OrganizadorId);

            if (organizador == null)
            {
                return NotFound("El organizador no existe");
            }

            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }

            if (usuario.seguidos.Any(e => e.OrganizadorId == organizador.OrganizadorId))
            {
                return BadRequest("Ya habias seguido al organizador");
            }

            usuario.seguidos.Add(organizador);
            context.SaveChanges();

            return Ok("Ya estas siguiendo al organizador");
        }

        

        [HttpPut("Editar usuario")]
        public async Task<ActionResult> Put(UsuarioDTO UsuarioDTO, int UsuarioId)
        {
            var exists = await context.Usuarios.AnyAsync(x => x.UsuarioId == UsuarioId);
            if (!exists)
            {
                return NotFound("El usuario no fue encontrado");
            }
            var usuario = mapper.Map<Usuario>(UsuarioDTO);
            usuario.UsuarioId = UsuarioId;

            context.Update(usuario);
            await context.SaveChangesAsync();
            return Ok(usuario);

        }

        [HttpDelete("Eliminar usuario")]
        public async Task<ActionResult> Delete(int UsuarioId)
        {
            var exists = await context.Usuarios.AnyAsync(x => x.UsuarioId == UsuarioId);
            if (!exists)
            {
                return NotFound("El usuario no fue encontrado");
            }

            context.Remove(new Usuario { UsuarioId = UsuarioId });
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpDelete("Eliminar evento de favoritos")]
        public IActionResult EliminarFavorito(int UsuarioId, int EventoId)
        {
            var usuario = context.Usuarios.Include(o => o.Favoritos).FirstOrDefault(o => o.UsuarioId == UsuarioId);
            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }
            var evento = usuario.Favoritos.FirstOrDefault(e => e.EventoId == EventoId);
            var eve = context.Eventos.FirstOrDefault(e => e.EventoId == EventoId);

            if (eve == null)
            {
                return NotFound("El evento no existe ");
            }

            if (evento == null)
            {
                return NotFound("El evento no fue encontrado en favoritos");
            }


            usuario.Favoritos.Remove(evento);
            context.SaveChanges();

            return Ok("Evento eliminado de favoritos exitosamente");
        }

        [HttpDelete("Dejar de seguir organizador")]
        public IActionResult DejardeSeguirOrganizador(int UsuarioId, int OrganizadorId)
        {
            var usuario = context.Usuarios.Include(o => o.seguidos).FirstOrDefault(o => o.UsuarioId == UsuarioId);
            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }
            var organizador = usuario.seguidos.FirstOrDefault(e => e.OrganizadorId == OrganizadorId);
            var orga = context.Organizadores.FirstOrDefault(e => e.OrganizadorId == OrganizadorId);

            if (orga == null)
            {
                return NotFound("El organizador no existe ");
            }

            if (organizador == null)
            {
                return NotFound("No sigues al organizador");
            }


            usuario.seguidos.Remove(organizador);
            context.SaveChanges();

            return Ok("Dejaste de seguir al organizador");
        }
    }
}
