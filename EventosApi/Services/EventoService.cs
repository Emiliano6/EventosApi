using EventosApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using EventosApi.Migrations;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EventosApi.Services
{
    public class EventoService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly ApplicationDbContext context;
        private SmtpClient smtpClient;
        public IConfiguration Configuration { get; }
        private readonly ILogger<EventoService> _logger;

        public EventoService( IConfiguration configuration, IServiceProvider serviceProvider, ILogger<EventoService> logger)
        {
            //this.context = context;
            _serviceProvider = serviceProvider;
            _logger = logger;
            Configuration = configuration;
            smtpClient = new SmtpClient("smtp.zoho.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["Email:Address"], Configuration["Email:Password"]),
                EnableSsl = true,
            };
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("El servicio EventoService ha comenzado.");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var eventos = context.Eventos.Include(a => a.Organizadores).ToList();
                    DateTime hoy = DateTime.Now;
                    DateTime mañana = DateTime.Now.AddDays(1);
                    foreach (var evento in eventos)
                    {
                        if (evento.Fecha == mañana && evento.CorreoEnviado == false)
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
                            evento.CorreoEnviado = true;
                        }
                    }
                    await context.SaveChangesAsync();
                }
                _logger.LogInformation("El servicio EventoService ha completado un ciclo de ejecución.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            _logger.LogInformation("El servicio EventoService ha finalizado.");
        }
    }
 }
    
