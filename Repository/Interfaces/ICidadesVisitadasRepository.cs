namespace TravelShare.Repository.Interfaces
{
    // Interface que define os métodos para manipulação de cidades visitadas por um usuário
    public interface ICidadesVisitadasRepository
    {
        // Método assíncrono que verifica se uma cidade foi visitada pelo usuário
        // 'usuarioId' é o identificador do usuário e 'cidade' é o nome da cidade a ser verificada
        // Retorna 'true' se a cidade já foi visitada pelo usuário, caso contrário, retorna 'false'
        Task<bool> VerificarCidadeVisitadaPeloUsuarioAsync(string usuarioId, string cidade);

        // Método assíncrono que adiciona uma cidade à lista de cidades visitadas pelo usuário
        // 'usuarioId' é o identificador do usuário e 'cidade' é o nome da cidade a ser adicionada
        // Retorna 'true' se a cidade foi adicionada com sucesso, caso contrário, retorna 'false'
        Task<bool> AddCidadeAsync(string usuarioId, string cidade);

        // Método assíncrono que remove uma cidade da lista de cidades visitadas pelo usuário
        // 'usuarioId' é o identificador do usuário e 'cidade' é o nome da cidade a ser removida
        // Retorna 'true' se a cidade foi removida com sucesso, caso contrário, retorna 'false'
        Task<bool> RemoveCidadeAsync(string usuarioId, string cidade);
    }
}
