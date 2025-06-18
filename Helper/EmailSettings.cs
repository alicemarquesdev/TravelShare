namespace TravelShare.Helper
{
    // Classe usada para armazenar as configurações do SMPT (Para o envio de emails).
    // A classe é configurada no Program.cs, onde as configurações do SMPT são lidas do appsettings.json 
    // e injetadas automaticamente através da injeção de dependência.
    // As chaves serão carregadas a partir do appsettings.json, onde estarão armazenadas de maneira segura.
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
    }
}
