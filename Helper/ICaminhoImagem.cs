namespace TravelShare.Helper
{
    public interface ICaminhoImagem
    {
        Task<string> GerarCaminhoImagemAsync(IFormFile imagem);
    }
}