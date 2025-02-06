namespace TravelShare.Helper
{
    public class CaminhoImagem : ICaminhoImagem
    {
        private readonly string _sistema;

        public CaminhoImagem(IWebHostEnvironment sistema)
        {
            _sistema = sistema.WebRootPath;
        }

        // Método para gerar caminho de arquivo de uma imagem
        public async Task<string> GerarCaminhoImagemAsync(IFormFile imagem)
        {
            // Caminho base para salvar as imagens, dentro da pasta "wwwroot/images"
            var caminhoParaSalvarImagem = Path.Combine(_sistema, "image");

            // Verifica se a pasta existe e, se não, cria
            if (!Directory.Exists(caminhoParaSalvarImagem))
            {
                Directory.CreateDirectory(caminhoParaSalvarImagem);
            }

            // Gera um código único para o arquivo
            var codigoUnico = Guid.NewGuid().ToString();

            // Obtém a extensão original do arquivo (ex: ".jpg", ".png")
            var extensao = Path.GetExtension(imagem.FileName).ToLower();

            // Gera o nome final do arquivo (sem espaços e com a extensão original)
            var nomeCaminhoImagem = codigoUnico + extensao;

            // Caminho completo para salvar a imagem
            var caminhoCompleto = Path.Combine(caminhoParaSalvarImagem, nomeCaminhoImagem);

            // Salva o arquivo no caminho especificado
            using (var stream = File.Create(caminhoCompleto))
            {
                await imagem.CopyToAsync(stream);
            }

            // Retorna o caminho relativo da imagem salva
            return Path.Combine("~/image", nomeCaminhoImagem).Replace("\\", "/");
        }
    }
}