using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;

namespace TravelShare.Helper
{
    // Classe que implementa a remoção de notificações antigas, removendo do banco de dados.
    public class NotificacaoCleanupService : BackgroundService
    {
        // Coleção de notificações que será utilizada para acessar e manipular os dados no MongoDB.
        private readonly IMongoCollection<NotificacaoModel> _notificacaoCollection;

        // Intervalo de tempo para a execução do serviço (uma vez a cada 24 horas).
        private readonly TimeSpan _intervalo = TimeSpan.FromHours(24);

        // Logger para logar informações e erros
        private readonly ILogger<NotificacaoCleanupService> _logger;

        // Construtor que recebe o contexto do MongoDB (MongoContext) e o serviço de logging (ILogger).
        public NotificacaoCleanupService(MongoContext context, ILogger<NotificacaoCleanupService> logger)
        {
            // Atribuindo a coleção de notificações que será limpa periodicamente.
            _notificacaoCollection = context.GetCollection<NotificacaoModel>("Notificacoes");

            // Inicializando o logger.
            _logger = logger;
        }

        // Método principal que executa o serviço em segundo plano. Ele roda em loop até que o serviço seja cancelado.
        // O método chama RemoverNotificacoesAntigas a cada 24 horas, verificando se há notificações antigas para remover.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Enquanto o serviço não for cancelado
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Chama o método que limpa notificações antigas
                    await RemoverNotificacoesAntigas();
                }
                catch (Exception ex)
                {
                    // Loga o erro, caso algo falhe durante a remoção das notificações
                    _logger.LogError(ex, "Erro ao remover notificações antigas");
                }

                // Aguarda o intervalo de 24 horas (ou até o serviço ser cancelado)
                await Task.Delay(_intervalo, stoppingToken);
            }
        }

        // Método que remove notificações antigas. Notificações com mais de 7 dias são consideradas antigas.
        private async Task RemoverNotificacoesAntigas()
        {
            try
            {
                // Definindo a data limite para notificações (7 dias atrás)
                var limiteData = DateTime.UtcNow.AddDays(-7);

                // Filtro para buscar notificações com data de criação anterior à data limite
                var filtro = Builders<NotificacaoModel>.Filter.Lt(n => n.DataCriacao, limiteData);

                // Deletando notificações antigas no MongoDB
                var resultado = await _notificacaoCollection.DeleteManyAsync(filtro);

                // Loga a quantidade de notificações removidas
                _logger.LogInformation($"🔄 {resultado.DeletedCount} notificações antigas removidas.");
            }
            catch (Exception ex)
            {
                // Loga o erro, caso algo falhe durante a execução da exclusão
                _logger.LogError(ex, "Erro ao remover notificações antigas do MongoDB");
            }
        }
    }
}