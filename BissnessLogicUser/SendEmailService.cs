using InformationSystemOfASchoolIducationalPortal.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class SendEmailService
    {
        private readonly EmailSettings _settings;
        public SendEmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public static OperationResult Ok(string message) => new OperationResult { Success = true, Message = message };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        public async Task<OperationResult> SendEmail(string toEmail, string parentName, string subject, string body)
        {
            if (toEmail == null)
                return OperationResult.Fail("Почта пуста, укажите почту пользователя");
            if (string.IsNullOrWhiteSpace(body))
                return OperationResult.Fail("Текст для отправки сообщения пуста");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Администарция школы", _settings.SmtpUser));
            message.To.Add(new MailboxAddress(parentName, toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };
            using var client = new SmtpClient();
            try
            {
                Console.WriteLine("CONNECT START");
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                Console.WriteLine("CONNECT OK");

                Console.WriteLine("AUTH START");
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword);
                Console.WriteLine("AUTH OK");

                Console.WriteLine("SEND START");
                await client.SendAsync(message);
                Console.WriteLine("SEND OK");

                await client.DisconnectAsync(true);
                return OperationResult.Ok("Сообщение отправлено");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SMTP ERROR:");
                Console.WriteLine(ex.ToString());
                return OperationResult.Fail(ex.ToString());
            }
        }

    }
}
