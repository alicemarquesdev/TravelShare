using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TravelShare.Helper;

public class Email : IEmail
{
    private readonly IConfiguration _configuration;

    public Email(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderPassword = _configuration["EmailSettings:SenderPassword"];

        using (var client = new SmtpClient(smtpServer, smtpPort))
        {
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = assunto,
                Body = mensagem,
                IsBodyHtml = true
            };

            mailMessage.To.Add(destinatario);

            await client.SendMailAsync(mailMessage);

            return true;
        }
    }
}
