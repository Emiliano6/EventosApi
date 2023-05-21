﻿namespace EventosApi.Controllers
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

        [HttpPost]
        public async Task<ActionResult<Organizador>> Post(Organizador organizador)
        {
            context.Add(organizador);
            await context.SaveChangesAsync();
            return Ok(organizador);
        }

        [HttpPut("{OrganizadorId:int}")]
        public async Task<ActionResult> Put(Organizador Organizador, int OrganizadorId)
        {
            var exists = await context.Organizadores.AnyAsync(x => x.OrganizadorId == OrganizadorId);
            if (!exists)
            {
                return NotFound("El organizador no fue encontrado");
            }
            if (Organizador.OrganizadorId != OrganizadorId)
            {
                return BadRequest("El id del organizador no coincide con el de la url");
            }
            context.Update(Organizador);
            await context.SaveChangesAsync();
            return Ok();

        }
        
        [HttpDelete("/{OrganizadorId}")]
        public async Task<ActionResult> Delete(int OrganizadorId)
        {
            var exists = await context.Organizadores.AnyAsync(x => x.OrganizadorId == OrganizadorId);
            if (!exists)
            {
                return NotFound("El organizador no fue encontrado");
            }

            context.Remove(new Organizador { OrganizadorId = OrganizadorId });
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet("Promociones")]
        public async Task<ActionResult<List<Promocion>>> GetPromociones()
        {
            return await context.Promociones.ToListAsync();
        }

        [HttpPost("AgregarPromociones")]
        public async Task<ActionResult<Promocion>> Post(Promocion Promocion)
        {
            context.Add(Promocion);
            await context.SaveChangesAsync();
            return Ok(Promocion);
        }

        [HttpDelete("EliminarPromociones")]
        public async Task<ActionResult> DeletePromocion(int PromocionId)
        {
            var exists = await context.Promociones.AnyAsync(x => x.PromocionId == PromocionId);

            if (!exists )
            {
                return NotFound("La promocion no fue encontrada");
            }

            context.Remove(new Promocion { PromocionId = PromocionId });
            context.SaveChanges();
            return Ok();

        }

    }
}
