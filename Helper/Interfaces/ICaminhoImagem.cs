namespace TravelShare.Helper.Interfaces
{
    // Declara o método necessário que irá gerar o caminho de armazenamento de imagens.
    // GerarCaminhoArquivoAsync recebe um arquivo de imagem (IFormFile), salva no servidor e retorna o caminho URL como string.
    // RemoverImagemAntiga remove a imagem antiga do servidor, dado o caminho da imagem.
    public interface ICaminhoImagem
    {
        // Método assíncrono para gerar o caminho do arquivo da imagem.
        // Recebe um arquivo de imagem (IFormFile), salva no servidor e retorna o caminho URL como string.
        Task<string> GerarCaminhoArquivoAsync(IFormFile imagem);

        // Método para remover a imagem antiga do servidor, dado o caminho da imagem.
        Task<bool> RemoverImagemAntiga(string caminhoImagemAntiga);
    }
}
