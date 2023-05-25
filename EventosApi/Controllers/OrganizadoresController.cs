namespace EventosApi.Controllers
{
    using AutoMapper;
    using EventosApi.Data;
    using EventosApi.DTOs;
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
        private readonly IMapper mapper;

        public OrganizadoresController(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            this.context = context;
            Configuration = configuration;
            this.mapper = mapper;
            smtpClient = new SmtpClient("smtp.zoho.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["Email:Address"], Configuration["Email:Password"]),
                EnableSsl = true,
            };
        }

        [HttpGet("Organizadores")]
        public async Task<ActionResult<List<GetOrganizadorDTO>>> Get()
        {
            var organizadores = context.Organizadores.Include(e=>e.Eventos).ToList();
            var organizadorDTO = mapper.Map<List<GetOrganizadorDTO>>(organizadores);
            return Ok(organizadorDTO);
        }

        [HttpGet("Promociones")]
        public async Task<ActionResult<List<GetPromocionDTO>>> GetPromociones()
        {
            var promociones = context.Promociones.ToList();
            var promocionesDTO = mapper.Map<List<GetPromocionDTO>>(promociones);
            return Ok(promocionesDTO);
        }

        [HttpPost("Agregar organizador")]
        public async Task<ActionResult<OrganizadorDTO>> Post(OrganizadorDTO organizadorDTO)
        {
            var organizador = mapper.Map<Organizador>(organizadorDTO);
            context.Add(organizador);
            await context.SaveChangesAsync();
            return Ok(organizador);
        }

        [HttpPost("Agregar promocion")]
        public async Task<ActionResult<PromocionDTO>> Post(PromocionDTO PromocionDTO)
        {

            var mailMessage = new MailMessage
            {
                From = new MailAddress(Configuration["Email:Address"]),
                Subject = string.Format("Hay una nueva promocion para el evento al que te registraste"),
                Body = string.Format("Hola, hay una nueva promocion, se llama {0} y tiene un descuento de {1}% sobre el total de tu pago.", PromocionDTO.CodigoPromocion, ((1 - PromocionDTO.descuento) * 100).ToString("0.00")),
                IsBodyHtml = false,
            };
            var usuarios = context.Eventos.Include(e => e.Registrados).FirstOrDefault(o => o.EventoId == PromocionDTO.EventoId);
            if (usuarios == null)
            {
                return NotFound("El evento que ingresaste no existe");
            }
            if (!usuarios.Registrados.Any())
            {
                return NotFound("Parece que aun no hay usuarios registrados en el evento");
            }
            foreach (var usuario in usuarios.Registrados)
            {
                mailMessage.To.Add(new MailAddress(usuario.Correo));
            }
            var promocion = mapper.Map<Promocion>(PromocionDTO);
            smtpClient.Send(mailMessage);
            context.Add(promocion);
            await context.SaveChangesAsync();
            return Ok(promocion);
        }

        [HttpPut("Editar organizador")]
        public async Task<ActionResult> Put(OrganizadorDTO OrganizadorDTO, int OrganizadorId)
        {
            var exists = await context.Organizadores.AnyAsync(x => x.OrganizadorId == OrganizadorId);
            if (!exists)
            {
                return NotFound("El organizador no fue encontrado");
            }
            var organizador = mapper.Map<Organizador>(OrganizadorDTO);
            organizador.OrganizadorId = OrganizadorId;

            context.Update(organizador);
            await context.SaveChangesAsync();
            return Ok();

        }
        
        [HttpDelete("Eliminar organizador")]
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

        [HttpDelete("Eliminar promocion")]
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
