using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using TiendaKeytlin.Server.Models;

namespace TiendaKeytlin.Server.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _smtpSettings = emailConfiguration.Value.SmtpSettings;
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            if (string.IsNullOrEmpty(emailModel.Email))
            {
                throw new ArgumentException("El correo electrónico no puede ser nulo o vacío", nameof(emailModel.Email));
            }

            if (string.IsNullOrEmpty(_smtpSettings.SenderEmail))
            {
                throw new ArgumentException("El correo electrónico del remitente no puede ser nulo o vacío", nameof(_smtpSettings.SenderEmail));
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", emailModel.Email));
            message.Subject = emailModel.Subject;
            message.Body = new TextPart("plain")
            {
                Text = emailModel.Message
            };

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpSettings.Server, _smtpSettings.Port, false);
                client.Authenticate(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }
    }
}