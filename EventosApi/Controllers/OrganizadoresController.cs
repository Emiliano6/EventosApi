namespace EventosApi.Controllers
{
    using EventosApi.Data;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    using System.Net;
    using System.Net.Mail;

    [ApiController]
    [Route("api/organizadores")]
    public class OrganizadoresController : ControllerBase{
        private readonly ApplicationDbContext context;
        public IConfiguration Configuration { get; }
        private SmtpClient smtpClient;

        public OrganizadoresController(ApplicationDbContext context, IConfiguration configuration)
        {
            this.context = context;
            Configuration = configuration;
            smtpClient = new SmtpClient("smtp.zoho.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["Email:Address"], Configuration["Email:Password"]),
                EnableSsl = true,
            };
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

            var mailMessage = new MailMessage
            {
                From = new MailAddress(Configuration["Email:Address"]),
                Subject = string.Format("Hay una nueva promocion para el evento al que te registraste"),
                Body = string.Format("Hola, hay una nueva promocion, se llama {0} y tiene un descuento de {1}% sobre el total de tu pago.", Promocion.CodigoPromocion,((1-Promocion.descuento)*100).ToString("0.00")),
                IsBodyHtml = false,
            };
            var usuarios = context.Eventos.Include(e=>e.Registrados).FirstOrDefault(o => o.EventoId == Promocion.EventoId);
            foreach (var usuario in usuarios.Registrados)
            {
                mailMessage.To.Add(new MailAddress(usuario.Correo));
            }

            smtpClient.Send(mailMessage);
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
