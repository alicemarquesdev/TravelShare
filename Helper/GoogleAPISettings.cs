namespace TravelShare.Helper
{
    // Classe usada para armazenar as configurações da chave API do Google
    // A classe é configurada no Program.cs, onde as configurações GoogleAPI são lidas do appsettings.json 
    // e injetadas automaticamente através da injeção de dependência.
    // A chave será carregada a partir do appsettings.json, onde estarão armazenadas de maneira segura.
    public class GoogleAPISettings
    {
        public string ApiKey { get; set; }
    }
}
