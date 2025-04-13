using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using TravelShare.Helper.Interfaces;

namespace TravelShare.Helper
{
    // A classe CaminhoImagem é responsável por lidar com a manipulação de arquivos de imagem,
    // - GerarCaminhoArquivoAsyns(IFormFile imagem): Salvando a imagem no servidor e retornando a URL da imagem salva.
    // Lida com a validação da extensão e tamanho do arquivo também.
    // - RemoverImagemAntiga(string caminhoImagemAntiga): Remove a imagem antiga do servidor, dado o caminho da imagem.
    public class CaminhoImagem : ICaminhoImagem
    {
        private readonly string _sistema;
        private readonly ILogger<CaminhoImagem> _logger;

        // O construtor recebe uma instância de IWebHostEnvironment, que fornece o caminho raiz do diretório do sistema,
        // usado para definir o local onde as imagens serão salvas (na pasta wwwroot/images).
        public CaminhoImagem(IWebHostEnvironment sistema, ILogger<CaminhoImagem> logger)
        {
            _sistema = sistema.WebRootPath; // Obtém o caminho raiz do diretório wwwroot.
            _logger = logger; // Responsável por logar mensagens e exceções.
        }

        // Este método é responsável por gerar o caminho do arquivo da imagem do produto.
        // Ele recebe a imagem, valida seu tamanho e extensão, gera um nome único para o arquivo e o salva no sistema de arquivos.
        public async Task<string> GerarCaminhoArquivoAsync(IFormFile imagem)
        {
            try
            {
                if (imagem == null)
                    throw new ArgumentNullException(nameof(imagem), "A imagem não pode ser nula.");

                const int TamanhoMaximoImagemMB = 2;
                const long TamanhoMaximoImagemBytes = TamanhoMaximoImagemMB * 1024 * 1024;

                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                var extensao = Path.GetExtension(imagem.FileName).ToLower();
                if (!extensoesPermitidas.Contains(extensao))
                    throw new InvalidDataException("Somente arquivos .jpg, .jpeg e .png são permitidos.");

                var codigoUnico = Guid.NewGuid().ToString();
                var nomeCaminhoImagem = $"{codigoUnico}{extensao}";
                var caminhoParaSalvarImagem = Path.Combine(_sistema, "image");

                if (!Directory.Exists(caminhoParaSalvarImagem))
                    Directory.CreateDirectory(caminhoParaSalvarImagem);

                var caminhoCompleto = Path.Combine(caminhoParaSalvarImagem, nomeCaminhoImagem);

                // Processamento da imagem (compressão e redimensionamento)
                using (var stream = imagem.OpenReadStream())
                using (var image = await Image.LoadAsync(stream))
                {
                    // Reduz a qualidade da imagem para reduzir o tamanho do arquivo
                    var encoder = new JpegEncoder { Quality = 80 }; // Ajuste a qualidade se necessário

                    // Se a imagem for muito grande, redimensiona para uma largura máxima de 1920px
                    if (image.Width > 1920)
                    {
                        image.Mutate(x => x.Resize(1920, 0));
                    }

                    // Salva a imagem processada garantindo que ela fique abaixo do limite
                    using (var outputStream = new MemoryStream())
                    {
                        await image.SaveAsync(outputStream, encoder);

                        // Verifica se ainda está acima do limite de 2MB
                        if (outputStream.Length > TamanhoMaximoImagemBytes)
                            throw new InvalidDataException("Não foi possível reduzir a imagem abaixo de 2MB.");

                        await File.WriteAllBytesAsync(caminhoCompleto, outputStream.ToArray());
                    }
                }

                return Path.Combine("~/image", nomeCaminhoImagem).Replace("\\", "/");
            }
            catch (ArgumentNullException ex)
            {
                // Loga o erro caso a imagem seja nula
                _logger.LogError(ex, "Erro: Imagem não pode ser nula.");
                throw;
            }
            catch (InvalidDataException ex)
            {
                // Loga o erro caso o formato ou tamanho da imagem seja inválido
                _logger.LogError(ex, "Erro: Formato de imagem inválido ou tamanho excedido.");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Loga qualquer erro inesperado
                _logger.LogError(ex, "Erro inesperado ao salvar a imagem.");
                throw new Exception("Ocorreu um erro ao salvar a imagem. Tente novamente mais tarde.");
            }
        }

        // Este método é responsável por remover uma imagem antiga do servidor, dado o caminho da imagem.
        public Task<bool> RemoverImagemAntiga(string caminhoImagemAntiga)
        {
            try
            {
                // Verificação se o caminho da imagem antiga é nulo ou vazio
                if (string.IsNullOrEmpty(caminhoImagemAntiga))
                {
                    _logger.LogError("O caminho da imagem antiga está nulo ou vazio.");
                    return Task.FromResult(false); // Retorna falso caso o caminho seja inválido
                }

                // Garante que o caminho da imagem seja gerado corretamente para o sistema de arquivos
                var caminhoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", caminhoImagemAntiga.TrimStart('~', '/'));

                // Ajusta as barras para o padrão do sistema operacional
                caminhoAntigo = caminhoAntigo.Replace("/", Path.DirectorySeparatorChar.ToString());

                // Verifica se o arquivo existe antes de tentar excluí-lo
                if (!System.IO.File.Exists(caminhoAntigo))
                {
                    _logger.LogError("Imagem não encontrada no caminho especificado.");
                    return Task.FromResult(false); // Retorna falso se a imagem não for encontrada
                }

                try
                {
                    // Exclui o arquivo de imagem
                    System.IO.File.Delete(caminhoAntigo);
                    return Task.FromResult(true); // Retorna verdadeiro caso a imagem seja excluída com sucesso
                }
                catch (IOException ex)
                {
                    // Loga erro caso ocorra um problema de IO ao excluir o arquivo
                    _logger.LogError(ex, "Erro de IO ao excluir a imagem antiga.");
                    return Task.FromResult(false);
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Loga erro caso ocorra um problema de permissão
                    _logger.LogError(ex, "Erro de permissão ao excluir a imagem antiga.");
                    return Task.FromResult(false);
                }
                catch (Exception ex)
                {
                    // Loga qualquer erro inesperado ao excluir o arquivo
                    _logger.LogError(ex, "Erro ao excluir a imagem antiga.");
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                // Loga erro geral de exceção
                _logger.LogError(ex, "Erro ao tentar remover a imagem antiga.");
                return Task.FromResult(false);
            }
        }
    }
}
