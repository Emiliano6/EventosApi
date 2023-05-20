using System;

namespace EventosApi.Controllers
{
    
    using EventosApi.Data;
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
    }
}
