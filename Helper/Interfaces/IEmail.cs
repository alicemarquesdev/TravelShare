namespace TravelShare.Helper.Interfaces
{ 
    // Declara o método necessário para enviar e-mails, como e-mails de redefinição de senha.
    public interface IEmail
    {
        // Método para enviar um e-mail.
        // Recebe o endereço de e-mail, mensagem e assunto como parâmetros.
        Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem);
    }
}
