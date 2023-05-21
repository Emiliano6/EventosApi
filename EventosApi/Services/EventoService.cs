using EventosApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EventosApi.Services
{
    public class EventoService
    {
        private readonly ApplicationDbContext context;

        public EventoService(ApplicationDbContext context)
        {
            this.context = context;
        }
        private void EnviarCorreosrecordatorios()
        {
            // Obtener el evento próximo en función de la fecha actual
            Evento eventoProximo = context.Eventos.Where(e => e.Fecha > DateTime.Now).OrderBy(e => e.Fecha).FirstOrDefault();

            if (eventoProximo == null)
            {
                return; // No hay eventos próximos
            }

            // Obtener la lista de participantes del evento próximo desde la base de datos
            List<Usuario> participantes = eventoProximo.Registrados.ToList();

            // Calcular la fecha de envío del correo (días antes del comienzo del evento)
            DateTime fechaEnvio = eventoProximo.Fecha.AddDays(-3); // Por ejemplo, se enviará el correo 3 días antes del evento

            // Verificar si la fecha de envío del correo ya ha pasado
            if (fechaEnvio < DateTime.Now)
            {
                return; // No se enviará el correo si la fecha ya ha pasado
            }

            // Preparar el correo electrónico
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Eventos UANL", "tucorreo@gmail.com")); // Dirección de correo electrónico del remitente
            foreach (var participante in participantes)
            {
                message.To.Add(new MailboxAddress(participante.Nombre, participante.Correo)); // Agregar la dirección de correo electrónico del participante como destinatario
            }
            message.Subject = "Recordatorio de evento: " + eventoProximo.Nombre_Evento; // Asunto del correo

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "Estimado participante, te recordamos que el evento '" + eventoProximo.Nombre_Evento + "' se llevará a cabo el día " + eventoProximo.Fecha.ToString("dd/MM/yyyy") + ". ¡Esperamos contar con tu presencia!"; // Cuerpo del correo
            message.Body = bodyBuilder.ToMessageBody();

            // Configurar el cliente SMTP y enviar el correo
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false); // Servidor SMTP que utilizarás (en este ejemplo, se utiliza Gmail)
                client.Authenticate("2023backend@gmail.com", "bBackend!"); // Credenciales de tu cuenta de correo electrónico
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
 }
    
