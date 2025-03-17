using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TravelShare.Data
{
    // Classe responsável por configurar o contexto de conexão com o banco MongoDB.
    public class MongoContext
    {
        // Instância do banco de dados MongoDB que será utilizada para interagir com as coleções.
        private readonly IMongoDatabase _database;

        // Construtor da classe MongoContext que recebe as configurações do MongoDB.
        // O IOptions é usado para acessar as configurações armazenadas no arquivo de configuração (appsettings.json).
        public MongoContext(IOptions<MongoSettings> mongoSettings)
        {
            // Cria o cliente MongoDB utilizando a string de conexão obtida da configuração.
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            // Obtém a instância do banco de dados especificado nas configurações.
            _database = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
        }

        // Método genérico para acessar qualquer coleção do banco de dados.
        // Retorna a coleção especificada pelo nome e tipo genérico T.
        public IMongoCollection<T> GetCollection<T>(string collectionname)
        {
            return _database.GetCollection<T>(collectionname);
        }

        // Classe responsável por armazenar as configurações necessárias para conectar ao MongoDB.
        // As configurações são lidas a partir do arquivo de configuração da aplicação.
        public class MongoSettings
        {
            // Nome do banco de dados que será usado na conexão.
            public string DatabaseName { get; set; }

            // String de conexão necessária para se conectar ao MongoDB.
            public string ConnectionString { get; set; }
        }
    }
}
