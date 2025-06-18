using System.Security.Cryptography;
using System.Text;

namespace TravelShare.Helper
{
    // Classe de criptografia que contém o método para gerar hashes, criptografando senhas antes de armazená-las no banco de dados
    public static class Criptografia
    {
        // Método de extensão para a classe string, que gera um hash usando o algoritmo SHA1
        // O 'this' é usado aqui para criar um método de extensão para a classe 'string'. Ex: senha.GerarHash()
        public static string GerarHash(this string valor)
        {
            // Criação de uma instância do algoritmo SHA1
            using (var hash = SHA1.Create())
            {
                // Codifica a string de entrada (valor) em um array de bytes
                var encoding = new ASCIIEncoding();
                var bytes = encoding.GetBytes(valor);

                // Computa o hash (resultado criptografado) a partir do array de bytes
                var hashBytes = hash.ComputeHash(bytes);

                // Criação de um StringBuilder para construir a string hexadecimal
                var strHexa = new StringBuilder();

                // Converte cada byte do hash para uma representação hexadecimal
                foreach (var byteValue in hashBytes)
                {
                    strHexa.Append(byteValue.ToString("x2"));
                }

                // Retorna o hash gerado como uma string no formato hexadecimal
                return strHexa.ToString();
            }
        }
    }
}
