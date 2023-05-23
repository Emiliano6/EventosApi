using EventosApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace EventosApi.Services
{
    public class EventoService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventoService> _logger;
        private SmtpClient smtpClient;

        public IConfiguration Configuration { get; }

        public EventoService(IServiceProvider serviceProvider, ILogger<EventoService> logger,IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            Configuration = configuration;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("El servicio EventoService ha comenzado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var eventos = context.Eventos
                        .Include(a => a.Organizadores)
                        .Include(e => e.Registrados)
                        .Where(e => e.Fecha.Date == DateTime.Today.AddDays(1) && !e.CorreoEnviado)
                        .ToList();

                    foreach (var evento in eventos)
                    {
                        var usuarios = evento.Registrados.Select(u => u.Correo).ToList();

                        if (usuarios.Any())
                        {
                            await EnviarCorreoAsync(evento, usuarios);
                        }
                        evento.CorreoEnviado = true;
                    }

                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("El servicio EventoService ha finalizado.");
        }

        private async Task EnviarCorreoAsync(Evento evento, List<string> destinatarios)
        {
            var smtpClient = new SmtpClient("smtp.zoho.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["Email:Address"], Configuration["Email:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(Configuration["Email:Address"]),
                Subject = "Evento próximo",
                Body = $"Hola,\n Recuerda que el evento {evento.Nombre_Evento} es mañana. ¡Te esperamos!",
                IsBodyHtml = false
            };

            foreach (var destinatario in destinatarios)
            {
                mailMessage.To.Add(new MailAddress(destinatario));
            }

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}