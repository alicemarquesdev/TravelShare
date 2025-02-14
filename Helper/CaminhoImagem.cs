using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace TravelShare.Helper
{
    public class CaminhoImagem : ICaminhoImagem
    {
        private readonly string _sistema;

        public CaminhoImagem(IWebHostEnvironment sistema)
        {
            _sistema = sistema.WebRootPath;
        }

        public async Task<string> GerarCaminhoImagemAsync(IFormFile imagem)
        {
            try
            {
                if (imagem == null || imagem.Length == 0)
                    throw new ArgumentException("Arquivo inválido ou vazio.");

                // Validação da extensão do arquivo (permite apenas imagens)
                var extensao = Path.GetExtension(imagem.FileName).ToLower();
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };

                if (!Array.Exists(extensoesPermitidas, e => e == extensao))
                    throw new ArgumentException("Formato de imagem não permitido. Apenas .jpg, .jpeg, .png.");

                // Diretório para salvar as imagens
                var caminhoParaSalvarImagem = Path.Combine(_sistema, "image");
                Directory.CreateDirectory(caminhoParaSalvarImagem); // Garante que a pasta existe

                // Gera um nome de arquivo seguro
                var codigoUnico = Guid.NewGuid().ToString();
                var nomeCaminhoImagem = $"{codigoUnico}{extensao}";
                var caminhoCompleto = Path.Combine(caminhoParaSalvarImagem, nomeCaminhoImagem);

                // Usando ImageSharp para redimensionar a imagem com limites de 600px a 1080px
                using (var image = await Image.LoadAsync(imagem.OpenReadStream()))
                {
                    // Limite de tamanho máximo e mínimo para a largura
                    int larguraMaxima = 1080;
                    int larguraMinima = 600;

                    // Verifica se a imagem precisa ser redimensionada
                    if (image.Width > larguraMaxima || image.Width < larguraMinima)
                    {
                        var novaLargura = image.Width > larguraMaxima ? larguraMaxima : image.Width < larguraMinima ? larguraMinima : image.Width;
                        var novaAltura = (int)(image.Height * ((float)novaLargura / image.Width));

                        // Redimensiona a imagem mantendo a proporção
                        image.Mutate(x => x.Resize(novaLargura, novaAltura));
                    }

                    // Salva a imagem redimensionada
                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await image.SaveAsync(stream, new JpegEncoder { Quality = 90 });
                    }
                }

                // Retorna o caminho relativo para ser usado no front-end
                return Path.Combine("~/image", nomeCaminhoImagem).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                // Log do erro (substituir por um logger real se possível)
                Console.WriteLine($"Erro ao salvar a imagem: {ex.Message}");
                return null;
            }
        }
    }
}