namespace TravelShare.Repository.Interfaces
{
    // Interface que define os métodos para manipulação de curtidas
    public interface ICurtidaRepository
    {
        // Método assíncrono que verifica se uma curtida já existe para um post de um usuário específico
        // 'postId' é o identificador do post que pode ter a curtida
        // 'usuarioId' é o identificador do usuário que pode ter curtido o post
        // Retorna 'true' se o usuário já curtiu o post, caso contrário, retorna 'false'
        Task<bool> BuscarCurtidaExistenteAsync(string postId, string usuarioId);

        // Método assíncrono que adiciona uma curtida a um post por um usuário específico
        // 'postId' é o identificador do post que será curtido
        // 'usuarioId' é o identificador do usuário que está curtindo o post
        // Retorna 'true' se a curtida foi adicionada com sucesso, caso contrário, retorna 'false'
        Task<bool> AddCurtidaAsync(string postId, string usuarioId);

        // Método assíncrono que remove a curtida de um post de um usuário específico
        // 'postId' é o identificador do post de onde a curtida será removida
        // 'usuarioId' é o identificador do usuário que deseja remover a curtida
        // Retorna 'true' se a curtida foi removida com sucesso, caso contrário, retorna 'false'
        Task<bool> RemoveCurtidaAsync(string postId, string usuarioId);
    }
}
