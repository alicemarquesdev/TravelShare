namespace TravelShare.Helper
{
    public interface IEmail
    {
        Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem);

    }
}
