using System;

namespace EventosApi.Controllers
{
    
    using EventosApi.Data;
    using EventosApi.Migrations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/Usuario")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public UsuariosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> Get()
        {
            return await context.Usuarios.ToListAsync();
        }
        

        [HttpPost]
        public async Task<ActionResult<Usuario>> Post(Usuario usuario)
        {
            context.Add(usuario);
            await context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPut("{UsuarioId:int}")]
        public async Task<ActionResult> Put(Usuario Usuario, int UsuarioId)
        {
            var exists = await context.Usuarios.AnyAsync(x => x.UsuarioId == UsuarioId);
            if (!exists)
            {
                return NotFound("El usuario no fue encontrado");
            }
            if (Usuario.UsuarioId != UsuarioId)
            {
                return BadRequest("El id del usuario no coincide con el de la url");
            }
            context.Update(Usuario);
            await context.SaveChangesAsync();
            return Ok(Usuario);

        }

        [HttpGet("Seguidos/{UsuarioId}")]
        public async Task<ActionResult<List<Evento>>> Get(int UsuarioId)
        {
            var usuario = await context.Usuarios.Include(u => u.Favoritos).FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);

            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }

            return usuario.Favoritos.ToList();
        }
    

        [HttpPost("Agregar Favoritos")]
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

        [HttpDelete("Eliminar Favoritos")]
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

        [HttpGet("Favoritos/{UsuarioId}")]
        public async Task<ActionResult<List<Organizador>>> Get(int UsuarioId,int us)
        {
            var usuario =  await context.Usuarios.Include(u => u.seguidos).FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);

            if (usuario == null)
            {
                return NotFound("El usuario no existe");
            }

            return usuario.seguidos.ToList();
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

        [HttpDelete("{UsuarioId:int}")]
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
    }
}
