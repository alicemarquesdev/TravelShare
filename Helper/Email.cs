using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TravelShare.Helper.Interfaces;

namespace TravelShare.Helper
{
    // A classe implementa a interface IEmail, responsável por enviar e-mails através de um servidor SMTP.
    public class Email : IEmail
    {
        private readonly string _smtpServer; // Endereço do servidor SMTP
        private readonly string _senderEmail; // Endereço de e-mail do remetente
        private readonly string _senderPassword; // Senha do e-mail do remetente
        private readonly int _smtpPort; // Porta SMTP para a conexão

        // Construtor da classe que recebe uma instância da configuração de EmailSMTP via injeção de dependência.
        public Email(IOptions<EmailSettings> emailSMPT)
        {
            _smtpServer = emailSMPT.Value.SmtpServer;
            _senderEmail = emailSMPT.Value.SenderEmail;
            _senderPassword = emailSMPT.Value.SenderPassword;
            _smtpPort = emailSMPT.Value.SmtpPort;
        }

        // Método que envia um e-mail de forma assíncrona. Recebe como parâmetros o destinatário, o assunto e a mensagem do e-mail.
        public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem)
        {
            try
            {
                // Recupera as configurações de SMTP 
                var smtpServer = _smtpServer; // Endereço do servidor SMTP
                var smtpPort = _smtpPort; // Porta do servidor SMTP
                var senderEmail = _senderEmail; // E-mail do remetente
                var senderPassword = _senderPassword; // Senha do e-mail do remetente

                // Configuração do cliente SMTP
                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    // Define as credenciais para autenticação no servidor SMTP
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    // Ativa o uso de SSL para uma conexão segura
                    client.EnableSsl = true;

                    // Criação da mensagem de e-mail
                    var mailMessage = new MailMessage
                    {
                        // Define o remetente (de onde o e-mail será enviado)
                        From = new MailAddress(senderEmail),

                        // Define o assunto do e-mail
                        Subject = assunto,

                        // Define o corpo do e-mail (mensagem)
                        Body = mensagem,

                        // Define que o corpo do e-mail será em HTML
                        IsBodyHtml = true
                    };

                    // Define o destinatário (para quem o e-mail será enviado)
                    mailMessage.To.Add(destinatario);

                    // Envia o e-mail de forma assíncrona
                    await client.SendMailAsync(mailMessage);

                    // Retorna true se o e-mail foi enviado com sucesso
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Lançando uma nova exceção personalizada com informações detalhadas
                throw new Exception("Erro ao enviar o e-mail. Verifique a configuração do servidor SMTP e os parâmetros fornecidos.", ex);
            }
        }
    }
}


